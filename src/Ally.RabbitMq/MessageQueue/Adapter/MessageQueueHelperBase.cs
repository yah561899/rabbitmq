using Ally.RabbitMq.MessageQueue.Exceptions;
using Ally.RabbitMq.MessageQueue.Models.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ally.RabbitMq.MessageQueue.Adapter
{
    /// <summary>
    /// MessageQueueHelperBase.
    /// </summary>
    public abstract class MessageQueueHelperBase
    {
        protected readonly List<MessageQueueOptions> config;


        public MessageQueueHelperBase(
            IConfiguration configuration)
        {
            this.config = configuration.GetSection("RabbitMq").Get<List<MessageQueueOptions>>();
            if (this.config.Count == 0)
            {
                throw new MessageQueueFactoryException(
                    MessageQueueFactoryException.ErrorType.GetMQAdapterNotFoundError);
            }
        }

        public MessageQueueOptions GetConfig(string instanceName)
        {
            var instanceConfigs = this.config.Where(w => w.Name == instanceName);

            if (instanceConfigs.Count() == 0)
            {
                throw new MessageQueueFactoryException(
                    MessageQueueFactoryException.ErrorType.GetMQAdapterNotFoundError);
            }

            if (instanceConfigs.Count() > 1)
            {
                throw new MessageQueueFactoryException(
                    MessageQueueFactoryException.ErrorType.GetMQAdapterDuplicateKeyError);
            }

            var instanceConfig = this.config.Where(w => w.Name == instanceName).First();
            return instanceConfig;
        }
    }
}
