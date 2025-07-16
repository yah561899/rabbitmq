namespace Ally.RabbitMq.MessageQueue.Adapter
{
    public interface IMessageQueuePublisherPoolHelper
    {
        IMessageQueuePublisher GetPublisher(string instanceName);

        void Release(string instanceName, IMessageQueuePublisher publisher);
    }
}
