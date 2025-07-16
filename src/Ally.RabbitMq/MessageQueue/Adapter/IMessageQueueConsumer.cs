using Ally.RabbitMq.MessageQueue.Models.Config;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Ally.RabbitMq.MessageQueue.Adapter
{
    public interface IMessageQueueConsumer : IDisposable
    {
        void Initialize(MessageQueueOptions config, IServiceScope scope);

        string QueueName { get; set; }

        void StartConsume(string queuename);
    }
}
