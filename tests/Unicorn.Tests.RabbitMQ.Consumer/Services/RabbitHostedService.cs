using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Unicorn.Integration.RabbitMQ;
using Unicorn.Integration.RabbitMQ.Enums;
using Unicorn.Integration.RabbitMQ.Interfaces;

namespace Unicorn.Tests.RabbitMQ.Consumer.Services;

internal sealed class RabbitHostedService : BackgroundService
{
    private readonly ILogger<RabbitHostedService> _logger;
    private readonly RabbitAction _action;
    private readonly RabbitSubscriber _subscriber;

    public RabbitHostedService(
        ILogger<RabbitHostedService> logger,
        IRabbitConnectionFactory connectionFactory,
        IServiceScopeFactory scopeFactory,
        RabbitAction action)
    {
        _logger = logger;
        _action = action;

        var serviceProvider = scopeFactory.CreateAsyncScope().ServiceProvider;
        _subscriber = new RabbitSubscriber(
            serviceProvider.GetRequiredService<ILogger<RabbitSubscriber>>(),
            connectionFactory,
            new[] { QueueExchanges.NewsDirect },
            QueueNames.Signals);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (stoppingToken.IsCancellationRequested)
        {
            _subscriber.Unsubscribe();
            return Task.CompletedTask;
        }

        _logger.LogInformation("Subscribing");

        _subscriber.Subscribe(_action.StartAsync);
        return Task.CompletedTask;
    }

    public override Task StopAsync(CancellationToken stoppingToken)
    {
        _subscriber.Unsubscribe();
        return base.StopAsync(stoppingToken);
    }
}
