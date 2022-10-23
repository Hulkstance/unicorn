using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using Unicorn.Integration.RabbitMQ;
using Unicorn.Integration.RabbitMQ.Interfaces;
using Unicorn.Logging;
using Unicorn.Tests.RabbitMQ.Consumer.Processors;
using Unicorn.Tests.RabbitMQ.Consumer.Services;

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

var host = new HostBuilder()
    .ConfigureAppConfiguration(c => c
        .AddEnvironmentVariables()
        .AddJsonFile($"appsettings.{environment}.json"))
    .ConfigureServices((context, services) =>
    {
        var factory = new ConnectionFactory
        {
            HostName = context.Configuration.GetValue<string>("RabbitMQOptions:Host")
        };
        services.AddSingleton<IRabbitConnectionFactory>(new RabbitConnectionFactory(factory));

        services.AddTransient<TradeProcessor>();

        services.AddSingleton<RabbitAction>();
        services.AddHostedService<RabbitHostedService>();
    })
    .ConfigureLogging()
    .UseConsoleLifetime()
    .Build();

await host.RunAsync();
