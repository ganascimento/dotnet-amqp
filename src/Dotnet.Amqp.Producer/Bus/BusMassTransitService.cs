using Dotnet.Amqp.Producer.Bus.Interfaces;
using MassTransit;

namespace Dotnet.Amqp.Producer.Bus;

public class BusMassTransitService : IBusMassTransitService
{
    private readonly IBus _bus;

    public BusMassTransitService(IBus bus, IConfiguration configuration)
    {
        _bus = bus;
    }

    public async Task Send(object body, string queue)
    {
        var uri = new Uri($"queue:{queue}");
        var endPoint = await _bus.GetSendEndpoint(uri);
        await endPoint.Send(body);
    }
}