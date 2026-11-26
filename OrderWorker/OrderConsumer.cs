using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderWorker.Data;
using OrderWorker.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace OrderWorker;

public class OrderConsumer : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OrderConsumer> _logger;
    private const string QueueName = "order_queue";

    public OrderConsumer(IServiceScopeFactory scopeFactory, ILogger<OrderConsumer> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            Port = 5672,
            UserName = "guest",
            Password = "guest"
        };

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(queue: QueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

        var received = new EventingBasicConsumer(channel);
        received.Received += async (model, ea) =>
        {
            var rawBody = ea.Body.ToArray();
            var orderIdStr = Encoding.UTF8.GetString(rawBody).Trim();

            await ProcessOrderAsync(orderIdStr, stoppingToken);
        };

        channel.BasicConsume(queue: QueueName, autoAck: false, consumer: received);

        await Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken);
    }

    private async Task ProcessOrderAsync(string orderIdStr, CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var order = await dbContext.Orders.FirstOrDefaultAsync(o => o.Id == Guid.Parse(orderIdStr), ct);

        if (order is null)
        {
            _logger.LogWarning("Order {OrderId} not found.", orderIdStr);
            return;
        }

        order.Status = "Processing";
        await dbContext.SaveChangesAsync(ct);

        _logger.LogInformation("Order {OrderId} status updated to Processing.", orderIdStr);

        await Task.Delay(TimeSpan.FromSeconds(3), ct);

        order.Status = "Completed";
        await dbContext.SaveChangesAsync(ct);

        _logger.LogInformation("Order {OrderId} status updated to Completed.", orderIdStr);
    }
}