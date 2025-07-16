using Ally.RabbitMq.MessageQueue.Models.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;
using System;

namespace Ally.RabbitMq.MessageQueue.Adapter
{
    public class MessageQueuePublisherPooledObjectPolicy : PooledObjectPolicy<IMessageQueuePublisher>
    {
        private readonly MessageQueueOptions config;
        private readonly IServiceProvider services;

        public MessageQueuePublisherPooledObjectPolicy(MessageQueueOptions config, IServiceProvider services)
        {
            this.config = config;
            this.services = services;
        }

        public override IMessageQueuePublisher Create()
        {
            var scope = this.services.CreateScope();
            var publisher = scope.ServiceProvider.GetRequiredService<IMessageQueuePublisher>();
            publisher.Initialize(this.config, scope);

            return publisher;
        }

        public override bool Return(IMessageQueuePublisher publisher)
        {
            if (publisher.Connection == null || !publisher.Connection.IsOpen ||
                publisher.Model == null || !publisher.Model.IsOpen)
            {
                publisher.Dispose(); 
                return false;
            }

            return true;
        }
    }
}
