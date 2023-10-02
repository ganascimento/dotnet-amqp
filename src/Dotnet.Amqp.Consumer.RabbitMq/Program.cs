using Dotnet.Amqp.Consumer.RabbitMq.Consumers;
using Dotnet.Amqp.Core.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

services.AddSingleton<IConfiguration>(builder);
services.InitializeCore(builder);
services.AddSingleton<PersonConsumer>();

var personConsumer = services.BuildServiceProvider().GetService<PersonConsumer>() ?? throw new InvalidOperationException("Reference not found!");

personConsumer.Start();