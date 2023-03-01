using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PrismaProject.Dto;
using RabbitMQ.Client;

namespace PrismaProject.AsyncDataServices;

public class MessageBusClient : IMessageBusClient
{
    private readonly ILogger<MessageBusClient> _logger;
    private readonly IConfiguration _configuration;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public MessageBusClient(ILogger<MessageBusClient> logger, IConfiguration configuration)
    {
        _configuration = configuration;
        _logger = logger;
        var HostName = _configuration["RabbitMQHost"];
        var Port = int.Parse(_configuration["RabbitMQPort"] ?? "5672");
        var UserName = _configuration["RabbitMQUserName"];
        var Password = _configuration["RabbitMQPassword"];
        _logger.LogInformation($"factory has built {HostName}, " +
                               $"{Port}, {UserName}, {Password}");
        var factory = new ConnectionFactory()
        {
            HostName = HostName,
            Port = Port,
            UserName = UserName,
            Password = Password
        };

        try
        {
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
            _logger.LogInformation("RabbitMQ connection success");
        }
        catch (Exception e)
        {
            _logger.LogError("Could not connect to message bus : {EMessage}", e.Message);
        }
    }

    private void RabbitMQ_ConnectionShutdown(object? sender, ShutdownEventArgs e)
    {
        _logger.LogInformation("RabbitMQ connection shutdown");
    }

    public void PublishNewMessage(MessagePublishDto msg)
    {
        var message = JsonSerializer.Serialize(msg);
        if (_connection.IsOpen)
        {
            _logger.LogInformation("RabbitMQ connection open, sending message...");
            SendMessage(message);
        }
        else
        {
            _logger.LogWarning("RabbitMQ connection closed, cant send message!");
        }
    }

    private void SendMessage(string msg)
    {
        var body = Encoding.UTF8.GetBytes(msg);
        _channel.BasicPublish(exchange: "trigger",
            routingKey: "",
            basicProperties: null,
            body: body);
        _logger.LogInformation("message sent {Msg}!", msg);
    }

    public void Dispose()
    {
        _logger.LogInformation("message bus disposed");
        if (_channel.IsOpen)
        {
            _channel.Close();
            _connection.Close();
        }
    }
}