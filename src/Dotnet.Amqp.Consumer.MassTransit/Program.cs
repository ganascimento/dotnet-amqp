using Dotnet.Amqp.Consumer.MassTransit.Consumers.Student;
using Dotnet.Amqp.Core.Configuration;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = new HostBuilder()
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        config
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        services.AddSingleton<IConfiguration>(context.Configuration);
        services.InitializeCore(context.Configuration);

        services.AddMassTransit(cfg =>
        {
            cfg.AddConsumer<CreateStudentConsumer>();
            cfg.AddConsumer<UpdateStudentConsumer>();
            cfg.AddConsumer<RemoveStudentConsumer>();

            cfg.UsingRabbitMq((ctx, busConfigurator) =>
            {
                busConfigurator.Host(context.Configuration.GetConnectionString("RabbitMQ"));
                busConfigurator.ConfigureEndpoints(ctx);

                busConfigurator.ReceiveEndpoint(context.Configuration["Queue:Student:Create"] ?? throw new InvalidOperationException("Queue not found!"), e =>
                {
                    e.ConfigureConsumer<CreateStudentConsumer>(ctx);
                });
            });
        });
    });

await builder.RunConsoleAsync();