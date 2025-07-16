using Ally.RabbitMq.MessageQueue.Exceptions;
using Ally.RabbitMq.MessageQueue.Models.Base;
using Ally.RabbitMq.MessageQueue.Models.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;

namespace Ally.RabbitMq.MessageQueue.Adapter
{
    public abstract class MessageQueueBase : IDisposable
    {
        private readonly ILogger<MessageQueueBase> logger;
        protected readonly MiddleConnectionFactory middleConnectionFactory;
        protected MessageQueueOptions Config;
        public IConnection Connection { get; private set; }
        public IModel Model { get; private set; }
        public IServiceScope Scope { get; private set; }

        private bool _disposed = false;

        public MessageQueueBase(MiddleConnectionFactory middleConnectionFactory)
        {
            this.middleConnectionFactory = middleConnectionFactory;
            this.Connection = middleConnectionFactory.CreateConnection();
            this.Model = Connection.CreateModel();
        }

        public void Initialize(MessageQueueOptions config, IServiceScope scope)
        {
            try
            {
                this.Config = config;
                this.InitialExchanges();
                this.InitialQueues();
                this.Scope = scope;
            }
            catch (Exception ex)
            {
                throw new MessageQueueConfigAdapterException(
                    MessageQueueConfigAdapterException.ErrorType.InitializeError,
                    ex,
                    config.Name);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                if (Model != null)
                {
                    Model.Close();
                    Model.Dispose();
                    Model = null;
                }

                if (Connection != null)
                {
                    Connection.Close();
                    Connection.Dispose();
                    Connection = null; 
                }

                if (Scope != null)
                {
                    Scope.Dispose();
                    Scope = null; 
                }
            }

            _disposed = true;
        }

        public IBasicProperties GetBasicProperties(int? priority)
        {
            var basicProperties = this.Model.CreateBasicProperties();
            if (priority != null)
            {
                basicProperties.Priority = Convert.ToByte(priority);
            }
            return basicProperties;
        }

        private void InitialExchanges()
        {
            foreach (var exchange in this.Config.Exchanges)
            {
                this.Model.ExchangeDeclare(
                    exchange: exchange.Name,
                    type: exchange.ExchangeType.ToString().ToLower());
            }
        }

        private void InitialQueues()
        {
            if (this.Config.Queues == null)
            {
                return;
            }
            foreach (var queue in this.Config.Queues)
            {
                if (queue.Multiple)
                {
                    queue.Name = $"{queue.Name}-{Guid.NewGuid()}";
                }
                this.Model.QueueDeclare(
                    queue: queue.Name,
                    durable: queue.Durable,
                    exclusive: queue.Exclusive,
                    autoDelete: queue.AutoDelete,
                    arguments: this.CreateArguments(queue));

                this.QueueBind(queue);
            }
        }

        private Dictionary<string, object> CreateArguments(Queue queue)
        {
            Dictionary<string, object> args = null;

            if (queue.Arguments != null)
            {
                args = new Dictionary<string, object>();
                args.Add("x-max-priority", queue.Arguments.MaxPriority);
            }

            return args;
        }

        private void QueueBind(Queue queue)
        {
            foreach (var routingKey in queue.RoutingKeys)
            {
                this.Model.QueueBind(
                    queue: queue.Name,
                    exchange: queue.ExchangeName,
                    routingKey: routingKey);
            }
        }
    }
}
