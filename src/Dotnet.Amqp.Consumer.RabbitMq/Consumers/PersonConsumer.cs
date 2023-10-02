using System.Text;
using System.Text.Json;
using Dotnet.Amqp.Core.Entities;
using Dotnet.Amqp.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Dotnet.Amqp.Consumer.RabbitMq.Consumers;

public class PersonConsumer
{
    private readonly IPersonCommandRepository _personCommandRepository;
    private readonly string _rabbitConnection;
    private readonly string _createPersonQueue;
    private readonly string _updatePersonQueue;
    private readonly string _removePersonQueue;

    public PersonConsumer(IPersonCommandRepository personCommandRepository, IConfiguration configuration)
    {
        _personCommandRepository = personCommandRepository;
        _rabbitConnection = configuration.GetConnectionString("RabbitMQ") ?? throw new InvalidOperationException("Invalid connection string!");
        _createPersonQueue = configuration["Queue:Person:Create"] ?? throw new InvalidOperationException("Queue not found!");
        _updatePersonQueue = configuration["Queue:Person:Update"] ?? throw new InvalidOperationException("Queue not found!");
        _removePersonQueue = configuration["Queue:Person:Remove"] ?? throw new InvalidOperationException("Queue not found!");
    }

    public void Start()
    {
        var factory = new ConnectionFactory { Uri = new Uri(_rabbitConnection) };

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        this.Create(channel);
        this.Update(channel);
        this.Delete(channel);

        Console.WriteLine("Press any key to exit.");
        Console.ReadKey();
    }

    private void Create(IModel channel)
    {
        channel.QueueDeclare(queue: _createPersonQueue, durable: true, exclusive: false, autoDelete: false);
        var consumer = new EventingBasicConsumer(channel);

        consumer.Received += async (model, ea) =>
        {
            Console.WriteLine("Start process Create Person!");

            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var personEntity = JsonSerializer.Deserialize<PersonEntity>(message);
            if (personEntity == null) return;

            await _personCommandRepository.CreateAsync(personEntity);

            Console.WriteLine("End process Create Person!");
        };

        channel.BasicConsume(queue: _createPersonQueue,
                                autoAck: true,
                                consumer: consumer);
    }

    private void Update(IModel channel)
    {
        channel.QueueDeclare(queue: _updatePersonQueue, durable: true, exclusive: false, autoDelete: false);
        var consumer = new EventingBasicConsumer(channel);

        consumer.Received += async (model, ea) =>
        {
            Console.WriteLine("Start process Update Person!");

            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var personEntity = JsonSerializer.Deserialize<PersonEntity>(message);
            if (personEntity == null) return;

            await _personCommandRepository.UpdateAsync(personEntity);

            Console.WriteLine("End process Update Person!");
        };

        channel.BasicConsume(queue: _updatePersonQueue,
                                autoAck: true,
                                consumer: consumer);
    }

    private void Delete(IModel channel)
    {
        channel.QueueDeclare(queue: _removePersonQueue, durable: true, exclusive: false, autoDelete: false);
        var consumer = new EventingBasicConsumer(channel);

        consumer.Received += async (model, ea) =>
        {
            Console.WriteLine("Start process Delete Person!");

            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var personEntity = JsonSerializer.Deserialize<PersonEntity>(message);
            if (personEntity == null) return;

            await _personCommandRepository.DeleteAsync(personEntity);

            Console.WriteLine("End process Delete Person!");
        };

        channel.BasicConsume(queue: _removePersonQueue,
                                autoAck: true,
                                consumer: consumer);
    }
}