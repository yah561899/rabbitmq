using Ally.RabbitMq.MessageQueue.Models;
using Ally.RabbitMq.MessageQueue.Models.Config;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using System;

namespace Ally.RabbitMq.MessageQueue.Adapter
{
    public interface IMessageQueuePublisher : IDisposable
    {
        IConnection Connection { get; }

        IModel Model { get; }

        void Initialize(MessageQueueOptions config, IServiceScope scope);

        void PushMessage(PublisherModel model, IBasicProperties basicProperties = null, bool isLastUse = true);
    }
}