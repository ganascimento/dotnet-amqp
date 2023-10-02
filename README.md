# Dotnet AMQP

Project developed to implement different types of AMQP in dotnet applications.

## Resources used

To develop this project, was used:

- DotNet 7
- RabbitMQ
- Kafka
- MassTransit

## What is AMQP?

AMQP, or Advanced Message Queuing Protocol, is an open-standard communication protocol designed for efficiently exchanging messages between different software systems or components. It acts as a middleware layer, facilitating reliable and asynchronous communication in distributed architectures.

AMQP defines a set of rules and conventions for message producers (senders) and consumers (receivers) to interact. Messages are sent to a message broker, which acts as an intermediary responsible for routing, storing, and delivering messages to their intended recipients.

## RabbitMQ

RabbitMQ is a widely-used open-source message broker software that implements the Advanced Message Queuing Protocol (AMQP). It serves as a messaging middleware, enabling efficient communication and coordination between different components or applications within a distributed system.

RabbitMQ functions as a message broker by receiving, storing, and routing messages between senders and receivers. It supports various messaging patterns, including publish-subscribe, request-reply, and work queues. Messages sent to RabbitMQ are stored in message queues until they are consumed by interested parties, allowing for asynchronous and reliable communication.

<p align="start">
  <img src="./assets/rabbitmq.png" width="100" />
</p>

## Kafka

Kafka is an open-source distributed streaming platform developed by the Apache Software Foundation. It is designed for ingesting, storing, processing, and transmitting large volumes of real-time data streams in a fault-tolerant and scalable manner.

At its core, Kafka operates as a publish-subscribe system, where data is organized into topics, and producers send messages to these topics. Consumers can then subscribe to these topics and receive the messages in real-time. Kafka's architecture is built around a distributed commit log, which provides high throughput and low-latency data handling.

<p align="start">
  <img src="./assets/kafka.png" width="100" style="background: #fff; padding: 2px 7px; border-radius: 5px" />
</p>

## Test

To run this project you need docker installed on your machine, see the docker documentation [here](https://www.docker.com/).

Having all the resources installed, run the command in a terminal from the root folder of the project and wait some seconds to build project image and download the resources:
`docker-compose up -d`

In terminal show this:

```console
[+] Running 2/2
 ✔ Network dotnet-rate-limiter_default  Created                0.8s
 ✔ Container rate_limiter_app           Started                1.2s
```

After this, access the link below:

- Swagger project [click here](http://localhost:5000/swagger)

### Stop Application

To stop, run: `docker-compose down`

# How implement

## RabbitMQ

To implement, first install the [RabbitMQ.Client](https://www.nuget.org/packages/RabbitMQ.Client/6.5.0).

### Producer

Create publish service:

```c#
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
```

To invoke, send like parameter the object message and the queue name:

```c#
_busRabbitService.Publish(data, "create_object_queue");
```

### Consumer

Create process message method:

```c#
private void Create(IModel channel)
{
    channel.QueueDeclare(queue: _createQueue, durable: true, exclusive: false, autoDelete: false);
    var consumer = new EventingBasicConsumer(channel);

    consumer.Received += async (model, ea) =>
    {
        Console.WriteLine("Start process Create!");

        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        var entity = JsonSerializer.Deserialize<ObjectEntity>(message);
        if (entity == null) return;

        await _commandRepository.CreateAsync(entity);

        Console.WriteLine("End process Create!");
    };

    channel.BasicConsume(queue: _createQueue,
                            autoAck: true,
                            consumer: consumer);
}
```

To start consumer, implement a <b>Start</b> method:

```c#
public void Start()
{
    var factory = new ConnectionFactory { Uri = new Uri(_rabbitConnection) };

    using var connection = factory.CreateConnection();
    using var channel = connection.CreateModel();

    this.Create(channel);

    Console.WriteLine("Press any key to exit.");
    Console.ReadKey();
}
```

## RabbitMQ with MassTransit

To implement, first install the packages:

- [MassTransit](https://www.nuget.org/packages/MassTransit).
- [MassTransit.RabbitMQ](https://www.nuget.org/packages/MassTransit.RabbitMQ)

### Producer

Configure in Program.cs

```c#
...
builder.Services.AddMassTransit(config =>
{
    config.UsingRabbitMq((ctx, busConfigurator) =>
    {
        busConfigurator.Host(builder.Configuration.GetConnectionString("RabbitMQ"));
    });
});
...
```

Implement publish service:

```c#
public class PublishMassTransitService<T> : IPublishMassTransitService<T> where T : class
{
    private readonly IPublishEndpoint _publishEndpoint;

    public PublishMassTransitService(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public async Task Publish(T body)
    {
        await _publishEndpoint.Publish<T>(body);
    }
}
```

Send message:

```c#
private readonly IPublishMassTransitService<ObjectDto> _publishObject;

public ObjectController(IPublishMassTransitService<ObjectDto> publishObject)
{
    _publishObject = publishObject;
}

[HttpPost()]
public ActionResult Invoke()
{
    ...
    _publishObject.Publish(objectMessage);

    return Ok();
}
```

### Consumer

Configure in Program.cs

```c#
...
services.AddMassTransit(cfg =>
{
    cfg.AddConsumer<ObjectConsumer>();

    cfg.UsingRabbitMq((ctx, busConfigurator) =>
    {
        busConfigurator.Host(context.Configuration.GetConnectionString("RabbitMQ"));
        busConfigurator.ConfigureEndpoints(ctx);
    });
});
...
```

Create consumer service:

```c#
public class ObjectConsumer : IConsumer<ObjectEntity>
{
    private readonly IObjectCommandRepository _objectCommandRepository;

    public ObjectConsumer(IObjectCommandRepository objectCommandRepository)
    {
        _objectCommandRepository = objectCommandRepository;
    }

    public async Task Consume(ConsumeContext<ObjectEntity> context)
    {
        Console.WriteLine("Start process Create!");

        await _objectCommandRepository.CreateAsync(context.Message);

        Console.WriteLine("End process Create!");
    }
}
```

## Kafka

To implement, first install the [Confluent.Kafka](https://www.nuget.org/packages/Confluent.Kafka).

### Producer

Implement publish service:

```c#
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
```

Send message:

```c#
...
_kafkaService.Produce("topic_create", dataMessage);
...
```

### Consumer

Implement consumer service:

```c#
public class ObjectConsumer : IHostedService
{
    private readonly string _connection;
    private readonly string _topic;
    private readonly IObjectCommandRepository _objectCommandRepository;

    public ObjectConsumer(
        IConfiguration configuration,
        IObjectCommandRepository objectCommandRepository)
    {
        _connection = configuration.GetConnectionString("Kafka") ?? throw new InvalidOperationException("Kafka connection not found!");
        _topic = configuration["Kafka:Topic:Object:create"] ?? throw new InvalidOperationException("Create topic not found!");
        _objectCommandRepository = objectCommandRepository;
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
                    var dto = JsonSerializer.Deserialize<CreateObjectDto>(consumeResult.Message.Value);

                    // Implement code
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
```

Configure in Program.cs

```c#
...
services.AddSingleton<IHostedService, ObjectConsumer>();
...
```
