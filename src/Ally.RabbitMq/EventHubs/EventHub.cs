using System;
using System.Collections.Generic;

namespace Ally.RabbitMq.EventHubs
{
    public class EventHub : IEventHub
    {
        private readonly Dictionary<string, Dictionary<Type, List<Action<object>>>> eventChannels;

        public EventHub()
        {
            this.eventChannels = new Dictionary<string, Dictionary<Type, List<Action<object>>>>();
        }

        public void Publish<TEvent>(string functionName, TEvent @event)
        {
            if (this.eventChannels.TryGetValue(functionName, out var channel))
            {
                var eventType = typeof(TEvent);
                if (channel.ContainsKey(eventType))
                {
                    var handlers = channel[eventType];
                    foreach (var handler in handlers)
                    {
                        handler(@event);
                    }
                }
            }
        }

        public void Subscribe<TEvent>(string channelKey, Action<TEvent> eventHandler)
        {
            if (!this.eventChannels.ContainsKey(channelKey))
            {
                this.eventChannels[channelKey] = new Dictionary<Type, List<Action<object>>>();
            }

            var channel = this.eventChannels[channelKey];

            var eventType = typeof(TEvent);
            if (!channel.ContainsKey(eventType))
            {
                channel[eventType] = new List<Action<object>>();
            }

            channel[eventType].Add((@event) => eventHandler((TEvent)@event));
        }
    }
}
