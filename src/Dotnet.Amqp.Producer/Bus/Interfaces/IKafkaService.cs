namespace Dotnet.Amqp.Producer.Bus.Interfaces;

public interface IKafkaService
{
    Task Produce(string topic, object message);
}