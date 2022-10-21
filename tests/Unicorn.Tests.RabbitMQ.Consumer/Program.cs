using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using Unicorn.Integration.RabbitMQ;
using Unicorn.Logging;
using Unicorn.Tests.RabbitMQ.Consumer.Processors;
using Unicorn.Tests.RabbitMQ.Consumer.Services;

var host = new HostBuilder()
    .ConfigureServices((context, services) =>
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost"
        };
        services.AddSingleton<IRabbitConnectionFactory>(new RabbitConnectionFactory(factory));

        services.AddTransient<TickerProcessor>();

        services.AddSingleton<RabbitAction>();
        services.AddHostedService<RabbitBackgroundService>();
    })
    .ConfigureLogging()
    .UseConsoleLifetime()
    .Build();

await host.RunAsync();
