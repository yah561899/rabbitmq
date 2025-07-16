using Ally.RabbitMq.EventHubs;
using Ally.RabbitMq.MessageQueue.Adapter;
using Ally.RabbitMq.MessageQueue.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Ally.RabbitMq.MessageQueue.Unit
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMqClient(
            this IServiceCollection services)
        {
            services.AddSingleton<IEventHub, EventHub>();
            services.AddSingleton<MiddleConnectionFactory>();
            services.AddSingleton<IMessageQueuePublisherPoolHelper, MessageQueuePublisherPoolHelper>();
            services.AddSingleton<IMessageQueueConsumerHelper, MessageQueueConsumerHelper>();
            services.AddTransient<IMessageQueuePublisher, MessageQueuePublisher>();
            services.AddTransient<IMessageQueueConsumer, MessageQueueConsumer>();
            return services;
        }

        public static IServiceCollection AddMqClient(
            this IServiceCollection services, Action<string> func)
        {
            services.AddMqClient();
            services.AddSingleton<Action<string>>(func);
            return services;
        }

        public static IServiceCollection AddMqClientV2(
            this IServiceCollection services, 
            Action<string> traceIdFunc,
            Action<string> allyTokenKeyFunc)
        {
            services.AddMqClient();
            services.AddSingleton(new NamedAction(RabbitMqConstant.TraceId, traceIdFunc));
            services.AddSingleton(new NamedAction(RabbitMqConstant.AllyTokenKey, allyTokenKeyFunc));
            return services;
        }

        public static IConfigurationBuilder AddMqConfig(this IConfigurationBuilder configuration)
        {
            return configuration
                    .AddJsonFile("Config/RabbitMqConfig.json", optional: false, reloadOnChange: true);
        }
    }
}
