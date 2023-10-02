using System.Text;
using System.Text.Json;
using Dotnet.Amqp.Producer.Bus.Interfaces;
using RabbitMQ.Client;

namespace Dotnet.Amqp.Producer.Bus;

public class BusRabbitService : IBusRabbitService
{
    private readonly string _rabbitConnection;

    public BusRabbitService(IConfiguration configuration)
    {
        _rabbitConnection = configuration.GetConnectionString("RabbitMQ") ?? throw new InvalidOperationException("Invalid connection string!");
    }

    public void Publish(object message, string queue)
    {
        var factory = new ConnectionFactory { Uri = new Uri(_rabbitConnection) };

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(queue: queue, durable: true, exclusive: false, autoDelete: false);

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

        channel.BasicPublish(exchange: "",
                             routingKey: queue,
                             mandatory: true,
                             basicProperties: null,
                             body: body);
    }
}