namespace Dotnet.Amqp.Producer.Bus.Interfaces;

public interface IBusMassTransitService
{
    Task Send(object body, string queue);
}