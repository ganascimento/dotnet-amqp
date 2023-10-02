using Dotnet.Amqp.Consumer.Kafka.Consumers;
using Dotnet.Amqp.Core.Configuration;
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

        services.AddSingleton<IHostedService, CreateTeacherConsumer>();
        services.AddSingleton<IHostedService, UpdateTeacherConsumer>();
        services.AddSingleton<IHostedService, RemoveTeacherConsumer>();
    });

await builder.RunConsoleAsync();