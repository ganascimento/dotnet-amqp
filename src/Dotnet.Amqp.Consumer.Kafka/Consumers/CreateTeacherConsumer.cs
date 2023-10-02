using System.Text.Json;
using Confluent.Kafka;
using Dotnet.Amqp.Core.Dtos.Teacher;
using Dotnet.Amqp.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Dotnet.Amqp.Consumer.Kafka.Consumers;

public class CreateTeacherConsumer : IHostedService
{
    private readonly string _connection;
    private readonly string _topic;
    private readonly ITeacherCommandRepository _teacherCommandRepository;

    public CreateTeacherConsumer(
        IConfiguration configuration,
        ITeacherCommandRepository teacherCommandRepository)
    {
        _connection = configuration.GetConnectionString("Kafka") ?? throw new InvalidOperationException("Kafka connection not found!");
        _topic = configuration["Kafka:Topic:Teacher:create"] ?? throw new InvalidOperationException("Create topic not found!");
        _teacherCommandRepository = teacherCommandRepository;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(() => ProcessAsync(cancellationToken));
        return Task.CompletedTask;
    }

    public async Task ProcessAsync(CancellationToken cancellationToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _connection,
            GroupId = $"{_topic}-group-0",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using (var consumer = new ConsumerBuilder<Null, string>(config).Build())
        {
            consumer.Subscribe(_topic);

            try
            {
                while (true)
                {
                    var consumeResult = consumer.Consume();
                    var dto = JsonSerializer.Deserialize<CreateTeacherDto>(consumeResult.Message.Value);
                    Console.WriteLine("Create", consumeResult.Message.Value);

                    if (dto == null) continue;

                    await _teacherCommandRepository.CreateAsync(new Core.Entities.TeacherEntity
                    {
                        PersonId = dto.PersonId,
                        Subject = dto.Subject
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Consumer error: {ex.Message}");
            }
            finally
            {
                consumer.Close();
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}