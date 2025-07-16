using Ally.RabbitMq.MessageQueue.Models;
using RabbitMQ.Client.Events;

namespace Ally.RabbitMq.MessageQueue.Models
{
    public class PublisherModel
    {
        public string ExchangeName { get; set; }

        public string RoutingKey { get; set; }

        public BasicDeliverEventArgs EventArgs { get; set; }

        public DataModel DataModel { get; set; }
    }
}
