using Ally.RabbitMq.MessageQueue.Models;
using RabbitMQ.Client.Events;

namespace Ally.RabbitMq.MessageQueue.Models
{
    public class ConsumerModel
    {
        public BasicDeliverEventArgs EventArgs { get; set; }

        public DataModel DataModel { get; set; }
    }
}
