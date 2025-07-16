using System;

namespace Ally.RabbitMq.MessageQueue.Models
{
    public class NamedAction
    {
        public string Name { get; }
        public Action<string> Action { get; }

        public NamedAction(string name, Action<string> action)
        {
            Name = name;
            Action = action;
        }
    }
}
