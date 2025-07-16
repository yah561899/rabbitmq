using Ally.RabbitMq.MessageQueue.Exceptions;
using System;
using System.IO;
using System.Text.Json;

namespace Ally.RabbitMq.MessageQueue.Config
{
    public class JsonConfigManager<T> : IConfigManager<T>
    {
        public T ReadConfig(string path)
        {
            if (File.Exists(path))
            {
                return this.Deserialize(path);
            }
            else
            {
                throw new FileNotFoundException($"Config file is not Exist, '{path}' ", path);
            }
        }

        public void WriteConfig(T obj, string path)
        {
            var setting = JsonSerializer.Serialize(obj);
            File.WriteAllText(path, setting);
        }

        private T Deserialize(string path)
        {
            try
            {
                var result = JsonSerializer.Deserialize<T>(File.ReadAllText(path));

                return result;
            }
            catch (Exception ex)
            {
                throw new JsonConfigManagerException(
                    JsonConfigManagerException.ErrorType.DeserializeError,
                    ex);
            }
        }
    }
}
