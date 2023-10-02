using Dotnet.Amqp.Producer.Bus.Interfaces;
using MassTransit;

namespace Dotnet.Amqp.Producer.Bus;

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