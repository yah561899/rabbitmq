using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Ally.RabbitMq.MessageQueue.Adapter
{
    /// <summary>
    /// MessageQueuePublisherPoolHelper.
    /// </summary>
    public class MessageQueuePublisherPoolHelper : MessageQueueHelperBase, IMessageQueuePublisherPoolHelper
    {
        private const int WaitInstanceInterval = 500;
        private readonly IServiceProvider services;
        private readonly Dictionary<string, ObjectPool<IMessageQueuePublisher>> idleInstances;
        private readonly ILogger<MessageQueuePublisherPoolHelper> logger;
        private readonly int maxRetryCount = 10;
        private readonly object _lock = new object();

        public MessageQueuePublisherPoolHelper(
            IServiceProvider services,
            ILogger<MessageQueuePublisherPoolHelper> logger,
            IConfiguration configuration)
            : base(configuration)
        {
            this.services = services;
            this.logger = logger;
            this.idleInstances = new Dictionary<string, ObjectPool<IMessageQueuePublisher>>();
        }

        public IMessageQueuePublisher GetPublisher(string instanceName)
        {
            IMessageQueuePublisher instance = null;
            if (!this.idleInstances.ContainsKey(instanceName))
            {
                lock (_lock)
                {
                    if (!this.idleInstances.ContainsKey(instanceName))
                    {
                        var instanceConfig = this.GetConfig(instanceName);
                        var policy = new MessageQueuePublisherPooledObjectPolicy(instanceConfig, this.services);
                        var objectPool = new DefaultObjectPool<IMessageQueuePublisher>( policy, instanceConfig.MaxConnection);
                        this.idleInstances.Add(instanceName, objectPool);
                    }
                }
            }

            var pool = this.idleInstances[instanceName];
            instance = pool.Get();
            int retryCount = 0;
            while (instance == null && retryCount < this.maxRetryCount)
            {
                this.logger.LogWarning($"Instance is null, retrying...");
                Thread.Sleep(WaitInstanceInterval);
                instance = pool.Get();
                retryCount++;
            }

            if (instance == null)
            {
                this.logger.LogError("Failed to get a valid instance from the pool after retries.");
                throw new InvalidOperationException("Failed to obtain a publisher instance.");
            }

            return instance;
        }


        public void Release(string instanceName, IMessageQueuePublisher publisher)
        {
            if (publisher == null)
            {
                this.logger.LogWarning("Release instance is null.");
                return;
            }

            if (!this.idleInstances.ContainsKey(instanceName))
            {
                this.logger.LogWarning($"InstanceName {instanceName} not found in idleInstances.");
                return;
            }

            var pool = this.idleInstances[instanceName];
            pool.Return(publisher);
        }
    }
}
