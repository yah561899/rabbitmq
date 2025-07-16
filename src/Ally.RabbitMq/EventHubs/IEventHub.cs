using System;

namespace Ally.RabbitMq.EventHubs
{
    public interface IEventHub
    {
        void Publish<TEvent>(string channelKey, TEvent @event);
        void Subscribe<TEvent>(string channelKey, Action<TEvent> eventHandler);
    }
}
