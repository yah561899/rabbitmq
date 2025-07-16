using Ally.RabbitMq.MessageQueue.Models.Enums;
using Ally.RabbitMq.MessageQueue.Unit;
using RabbitMQ.Client;
using System;
using Microsoft.Extensions.Logging;

public class MiddleConnectionFactory
{
    private readonly ILogger<MiddleConnectionFactory> _logger;
    private readonly ConnectionFactory _connectionFactory; // RabbitMQ 的 ConnectionFactory
    private bool _disposed = false;

    public MiddleConnectionFactory(ILogger<MiddleConnectionFactory> logger)
    {
        _logger = logger;

        _connectionFactory = new ConnectionFactory()
        {
            HostName = EnvironmentVariableReader<EnumRabbitMqEnvironmentVariable>.Get(EnumRabbitMqEnvironmentVariable.RabbitMqHost),
            UserName = EnvironmentVariableReader<EnumRabbitMqEnvironmentVariable>.Get(EnumRabbitMqEnvironmentVariable.RabbitMqUserName),
            Password = EnvironmentVariableReader<EnumRabbitMqEnvironmentVariable>.Get(EnumRabbitMqEnvironmentVariable.RabbitMqPassword),
            Port = Convert.ToInt32(EnvironmentVariableReader<EnumRabbitMqEnvironmentVariable>.Get(EnumRabbitMqEnvironmentVariable.RabbitMqPort)),
            AutomaticRecoveryEnabled = true,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(5)
        };
    }

    public IConnection CreateConnection()
    {
        try
        {
            var connection = _connectionFactory.CreateConnection();
            _logger.LogInformation("Created new RabbitMQ connection");
            return connection;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to create new RabbitMQ connection: {ex.Message}");
            throw;
        }
    }
}
