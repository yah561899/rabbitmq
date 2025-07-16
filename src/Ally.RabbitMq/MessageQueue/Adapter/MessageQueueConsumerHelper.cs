using Ally.RabbitMq.MessageQueue.Models.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Ally.RabbitMq.MessageQueue.Adapter
{
    /// <summary>
    /// MessageQueueConsumerHelper
    /// </summary>
    public class MessageQueueConsumerHelper : MessageQueueHelperBase, IMessageQueueConsumerHelper, IDisposable
    {
        private const int FailedRetryTime = 3;

        private readonly ILogger<MessageQueueConsumerHelper> logger;
        private readonly IServiceProvider services;
        private readonly Dictionary<string, IMessageQueueConsumer> mqInstances;
        private readonly Dictionary<Guid, (int retryCount, DateTime expirationTime)> retryMessage;

        private readonly TimeSpan retryTimeout;

        public MessageQueueConsumerHelper(
            ILogger<MessageQueueConsumerHelper> logger,
            IConfiguration configuration,
            IServiceProvider services)
            : base(configuration)
        {
            this.logger = logger;
            this.services = services;
            this.mqInstances = new Dictionary<string, IMessageQueueConsumer>();
            this.retryMessage = new Dictionary<Guid, (int retryCount, DateTime expirationTime)>();
            this.retryTimeout = TimeSpan.FromMinutes(5);
        }

        /// <summary>
        /// 釋放資源.
        /// </summary>
        public void Dispose()
        {
            foreach (var key in this.mqInstances.Keys)
            {
                this.mqInstances[key].Dispose();
            }

            retryMessage.Clear();
        }

        /// <summary>
        /// 啟動消費者.
        /// </summary>
        /// <param name="instanceName">實例名稱</param>
        /// <param name="config">可選的配置</param>
        public void StartConsumer(string instanceName, MessageQueueOptions config = null)
        {
            if (config == null)
            {
                config = this.GetConfig(instanceName);
            }

            if (!this.mqInstances.ContainsKey(instanceName))
            {
                var scope = this.services.CreateScope();
                var messageQueueConsumer = scope.ServiceProvider.GetRequiredService<IMessageQueueConsumer>();
                messageQueueConsumer.Initialize(config, scope);

                foreach (var queue in config.Queues)
                {
                    messageQueueConsumer.StartConsume(queue.Name);
                }

                this.mqInstances.Add(instanceName, messageQueueConsumer);
            }
        }

        /// <summary>
        /// 增加消息的重試次數，並檢查是否超過最大重試次數.
        /// </summary>
        /// <param name="correlationId">消息的唯一標識符 (GUID)</param>
        /// <returns>如果允許重試，返回 true；如果達到最大重試次數，返回 false</returns>
        public bool AddRetryTimes(Guid correlationId)
        {
            this.CleanupExpiredRetries();

            if (retryMessage.ContainsKey(correlationId))
            {
                var (retryCount, _) = retryMessage[correlationId];

                if (retryCount >= FailedRetryTime)
                {
                    return false;
                }

                retryMessage[correlationId] = (retryCount + 1, DateTime.UtcNow.Add(retryTimeout));
                return true;
            }
            else
            {
                retryMessage.Add(correlationId, (1, DateTime.UtcNow.Add(retryTimeout)));
                return true;
            }
        }

        /// <summary>
        /// 檢查條目是否已過期.
        /// </summary>
        /// <param name="expirationTime">條目的過期時間</param>
        /// <returns>如果條目已過期，返回 true；否則返回 false</returns>
        private bool IsExpired(DateTime expirationTime)
        {
            return DateTime.UtcNow > expirationTime;
        }

        /// <summary>
        /// 清理過期的重試記錄.
        /// </summary>
        public void CleanupExpiredRetries()
        {
            var keysToRemove = new List<Guid>();

            foreach (var kvp in retryMessage)
            {
                if (IsExpired(kvp.Value.expirationTime))
                {
                    keysToRemove.Add(kvp.Key);
                }
            }

            foreach (var key in keysToRemove)
            {
                retryMessage.Remove(key);
            }

            logger?.LogInformation($"Cleaned up {keysToRemove.Count} expired retry records.");
        }
    }
}
