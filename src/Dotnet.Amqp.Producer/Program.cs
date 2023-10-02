using Dotnet.Amqp.Core.Configuration;
using Dotnet.Amqp.Producer.Bus;
using Dotnet.Amqp.Producer.Bus.Interfaces;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.InitializeCore(builder.Configuration);

builder.Services.AddScoped<IBusRabbitService, BusRabbitService>();
builder.Services.AddScoped<IBusMassTransitService, BusMassTransitService>();
builder.Services.AddScoped(typeof(IPublishMassTransitService<>), typeof(PublishMassTransitService<>));
builder.Services.AddScoped<IKafkaService, KafkaService>();

builder.Services.AddMassTransit(config =>
{
    config.UsingRabbitMq((ctx, busConfigurator) =>
    {
        busConfigurator.Host(builder.Configuration.GetConnectionString("RabbitMQ"));
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
