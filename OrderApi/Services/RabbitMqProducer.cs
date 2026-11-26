using RabbitMQ.Client;
using System.Text;

namespace OrderApi.Services;

public class RabbitMqProducer : IDisposable
{
    private readonly string _hostName = "localhost";
    private readonly int _port = 5672;
    private readonly string _userName = "guest";
    private readonly string _password = "guest";

    public void PublishMessage(string queueName, string message)
    {
        var factory = new ConnectionFactory
        {
            HostName = _hostName,
            Port = _port,
            UserName = _userName,
            Password = _password
        };

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(queue: queueName,
                             durable: false,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);

        var body = Encoding.UTF8.GetBytes(message);
        channel.BasicPublish(exchange: string.Empty,
                             routingKey: queueName,
                             body: body);
    }

    public void Dispose()
    {
        // Nothing to dispose because connection is created and released per call inside PublishMessage.
    }
}