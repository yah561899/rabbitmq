using Ally.RabbitMq.MessageQueue.Models.Enums;

namespace Ally.RabbitMq.MessageQueue.Models.Base
{
    public class Exchange
    {
        public string Name { get; set; }

        public ExchangeType ExchangeType { get; set; } = ExchangeType.Topic;

        public bool Durable { get; set; }

        public bool AutoDelete { get; set; }
    }
}
