using System.Text.Json;
using Confluent.Kafka;
using Dotnet.Amqp.Producer.Bus.Interfaces;

namespace Dotnet.Amqp.Producer.Bus;

public class KafkaService : IKafkaService
{
    private readonly string _connection;

    public KafkaService(IConfiguration configuration)
    {
        _connection = configuration.GetConnectionString("Kafka") ?? throw new InvalidOperationException("Kafka connection not found!");
    }

    public async Task Produce(string topic, object message)
    {
        try
        {
            var config = new ProducerConfig
            {
                BootstrapServers = _connection
            };

            using (var producer = new ProducerBuilder<Null, string>(config).Build())
            {
                var result = await producer.ProduceAsync(
                    topic,
                    new Message<Null, string>
                    {
                        Value = JsonSerializer.Serialize(message)
                    }
                );
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Kafka error: {ex.Message}");
        }
    }
}