namespace Dotnet.Amqp.Producer.Bus.Interfaces;

public interface IPublishMassTransitService<T> where T : class
{
    Task Publish(T body);
}