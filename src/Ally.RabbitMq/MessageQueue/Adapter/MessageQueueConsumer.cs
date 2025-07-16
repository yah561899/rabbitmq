using Ally.RabbitMq.EventHubs;
using Ally.RabbitMq.MessageQueue.Exceptions;
using Ally.RabbitMq.MessageQueue.Models;
using Ally.RabbitMq.MessageQueue.Unit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ally.RabbitMq.MessageQueue.Adapter
{
    public class MessageQueueConsumer : MessageQueueBase, IMessageQueueConsumer
    {
        public string QueueName { get; set; }
        public IServiceScope Scope { get; set; }

        private readonly ILogger<MessageQueueConsumer> logger;
        private readonly IEventHub eventHub;
        private readonly Action<string> SetTracId;
        private readonly Action<string> SetAllyTokenKey;
        private EventingBasicConsumer eventingBasicConsumer;
        private IMessageQueueConsumerHelper messageQueueConsumerHelper;
        private readonly JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };

        public MessageQueueConsumer(
            ILogger<MessageQueueConsumer> logger,
            IEventHub eventHub,
            IMessageQueueConsumerHelper messageQueueConsumerHelper,
            MiddleConnectionFactory middleConnectionFactory,
            IEnumerable<NamedAction> namedActions = null,
            Action<string> SetTracId = null)
            : base(middleConnectionFactory)
        {
            this.logger = logger;
            this.eventHub = eventHub;
            this.messageQueueConsumerHelper = messageQueueConsumerHelper;
            if (SetTracId != null)
            {
                this.SetTracId = SetTracId;
            }
            else
            {
                this.SetTracId = namedActions.FirstOrDefault(na => na.Name == RabbitMqConstant.TraceId)?.Action;
                this.SetAllyTokenKey = namedActions.FirstOrDefault(na => na.Name == RabbitMqConstant.AllyTokenKey)?.Action;
            }
        }

        public void SetupServiceScope(IServiceScope serviceScope)
        {
            Scope = serviceScope;
        }

        /// <summary>
        /// StartConsume.
        /// </summary>
        /// <param name="queuename"> queuename.</param>
        public void StartConsume(string queuename)
        {
            this.QueueName = queuename;
            var queue = this.Config.Queues.First(w => w.Name == this.QueueName);

            this.RegisterReceived();

            this.Model.BasicConsume(
                queue: queue.Name,
                autoAck: queue.AutoAck,
                consumer: this.eventingBasicConsumer);
        }

        /// <summary>
        /// RegisterReceived.
        /// </summary>
        private void RegisterReceived()
        {
            try
            {
                this.eventingBasicConsumer = new EventingBasicConsumer(this.Model);
                this.eventingBasicConsumer.Received += (model, ea) =>
                {
                    HandleReceivedEvent(ea);
                };
            }
            catch (Exception ex)
            {
                throw new MessageQueueConfigAdapterException(
                    MessageQueueConfigAdapterException.ErrorType.RegisterReceivedError,
                    ex);
            }
        }

        private void HandleReceivedEvent(BasicDeliverEventArgs ea)
        {
            Guid correlationId = new Guid();
            string serializedDataModel = string.Empty;
            try
            {
                var serializedEventArgs = JsonConvert.SerializeObject(ea, jsonSerializerSettings);
                serializedDataModel = Encoding.UTF8.GetString(ea.Body.ToArray());

                string serializedBasicProperties = ea.BasicProperties != null
                    ? JsonConvert.SerializeObject(ea.BasicProperties, jsonSerializerSettings)
                    : "null";

                this.logger.LogInformation(
                    "(Origin) Ally MQ Rabbit MQ Received Message:\n" +
                    $"Data Model: {serializedDataModel}"
                );

                ConsumerModel dataExchangeModel = new ConsumerModel()
                {
                    DataModel = JsonConvert.DeserializeObject<DataModel>(serializedDataModel),
                    EventArgs = ea,
                };

                this.SetTracId(dataExchangeModel.DataModel.TraceId.ToString());

                this.logger.LogInformation(
                    "Ally MQ Rabbit MQ Received Message:\n" +
                    $"Exchange: {ea.Exchange}\n" +
                    $"RoutingKey: {ea.RoutingKey}\n" +
                    $"BasicProperties: {serializedBasicProperties}\n" +
                    $"Serialized Event Args: {serializedEventArgs}\n" +
                    $"Data Model: {serializedDataModel}"
                );

                if (this.SetAllyTokenKey != null)
                {
                    this.SetAllyTokenKey(dataExchangeModel.DataModel.AllyTokenKey);
                }

                this.eventHub.Publish($"{dataExchangeModel.DataModel.FunctionName}", dataExchangeModel);
                string correlationIdString = dataExchangeModel.EventArgs.BasicProperties.CorrelationId;

                if (string.IsNullOrEmpty(correlationIdString) || !Guid.TryParse(correlationIdString, out correlationId))
                {
                    correlationId = Guid.Empty;
                }

                this.Model.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                MessageQueueConfigAdapterException newex;
                if (this.messageQueueConsumerHelper.AddRetryTimes(correlationId))
                {
                    this.Model.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                    newex = new MessageQueueConfigAdapterException(
                        MessageQueueConfigAdapterException.ErrorType.PublishError,
                        ex,
                        serializedDataModel);
                }
                else
                {
                    this.Model.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    newex = new MessageQueueConfigAdapterException(
                        MessageQueueConfigAdapterException.ErrorType.ReceivedOverTimesError,
                        ex,
                        serializedDataModel);
                }
                this.logger.LogError(newex, newex.Message);
                throw newex;
            }
        }
    }
}
