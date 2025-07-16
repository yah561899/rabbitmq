using Ally.RabbitMq.MessageQueue.Models.Config;
using System;

namespace Ally.RabbitMq.MessageQueue.Adapter
{
    public interface IMessageQueueConsumerHelper : IDisposable
    {
        void StartConsumer(string instanceName, MessageQueueOptions config = null);

        bool AddRetryTimes(Guid correlationId);
    }
}