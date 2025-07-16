using Ally.RabbitMq.MessageQueue.Exceptions;
using Ally.RabbitMq.MessageQueue.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text;

namespace Ally.RabbitMq.MessageQueue.Adapter
{
    public class MessageQueuePublisher : MessageQueueBase, IMessageQueuePublisher
    {
        private readonly ILogger<MessageQueueBase> logger;
        private readonly IMessageQueuePublisherPoolHelper messageQueuePublisherPoolHelper;
        private bool _disposed = false;

        IConnection IMessageQueuePublisher.Connection => base.Connection;
        IModel IMessageQueuePublisher.Model => base.Model;

        private readonly JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore // 忽略 null 值
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageQueuePublisher"/> class.
        /// </summary>
        /// <param name="logger">logger.</param>
        public MessageQueuePublisher(
            ILogger<MessageQueueBase> logger,
            MiddleConnectionFactory middleConnectionFactory,
            IMessageQueuePublisherPoolHelper messageQueuePublisherPoolHelper)
            : base(middleConnectionFactory)
        {
            this.logger = logger;
            this.messageQueuePublisherPoolHelper = messageQueuePublisherPoolHelper;
        }

        /// <summary>
        /// PushMessage.
        /// </summary>
        /// <param name="model">SendModel</param>
        /// <param name="basicProperties">IBasicProperties</param>
        /// <param name="isLastUse">是否是最後一次使用</param>
        public void PushMessage(PublisherModel model, IBasicProperties basicProperties = null, bool isLastUse = true)
        {
            try
            {
                var stringData = JsonConvert.SerializeObject(model.DataModel, jsonSerializerSettings);
                var body = Encoding.UTF8.GetBytes(stringData);

                this.Model.BasicPublish(
                    exchange: model.ExchangeName,
                    routingKey: model.RoutingKey,
                    basicProperties: basicProperties,
                    body: body);

                string serializedBasicProperties = basicProperties != null
                    ? JsonConvert.SerializeObject(basicProperties, jsonSerializerSettings)
                    : "null";

                this.logger.LogInformation(
                    "Ally MQ Rabbit MQ Push Message:\n" +
                    $"Exchange: {model.ExchangeName}\n" +
                    $"RoutingKey: {model.RoutingKey}\n" +
                    $"BasicProperties: {serializedBasicProperties}\n" +
                    $"Data: {stringData}"
                );
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error during message publishing");
                throw new MessageQueueConfigAdapterException(
                    MessageQueueConfigAdapterException.ErrorType.PushMessageError,
                    ex);
            }
            finally
            {
                if (isLastUse && !_disposed)
                {
                    messageQueuePublisherPoolHelper.Release(Config.Name, this);
                }
            }
        }

        /// <summary>
        /// Dispose 資源
        /// </summary>
        public new void Dispose()
        {
            if (!_disposed)
            {
                base.Dispose();
                _disposed = true;
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~MessageQueuePublisher()
        {
            Dispose();
        }
    }
}
