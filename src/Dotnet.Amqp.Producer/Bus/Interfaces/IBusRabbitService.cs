namespace Dotnet.Amqp.Producer.Bus.Interfaces;

public interface IBusRabbitService
{
    void Publish(object body, string queue);
}