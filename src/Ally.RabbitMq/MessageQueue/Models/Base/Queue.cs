using System;
using System.Collections.Generic;
using System.Text;

namespace Ally.RabbitMq.MessageQueue.Models.Base
{
    public class Queue
    {
        public string Name { get; set; }

        public string ExchangeName { get; set; } = string.Empty;

        public List<string> RoutingKeys { get; set; } = new List<string>();

        public bool Durable { get; set; }

        public bool Exclusive { get; set; }

        public bool AutoDelete { get; set; }

        public bool AutoAck { get; set; }

        public bool Multiple { get; set; } = false;

        public QueueArguments Arguments { get; set; } = null;
    }
}
