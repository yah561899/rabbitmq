using Ally.RabbitMq.MessageQueue.Models.Base;

namespace Ally.RabbitMq.MessageQueue.Models.Config
{
    /// <summary>
    /// Configuration for RabbitMQ.
    /// </summary>
    public class MessageQueueOptions
    {
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Exchanges
        /// </summary>
        public Exchange[] Exchanges { get; set; } = null;

        /// <summary>
        /// Queues
        /// </summary>
        public Queue[] Queues { get; set; } = null;

        /// <summary>
        /// MaxConnection
        /// </summary>
        public int MaxConnection { get; set; } = 10;
    }


}
