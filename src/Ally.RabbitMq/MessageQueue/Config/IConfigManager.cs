namespace Ally.RabbitMq.MessageQueue.Config
{
    public interface IConfigManager<T>
    {
        T ReadConfig(string path);

        void WriteConfig(T obj, string path);
    }
}
