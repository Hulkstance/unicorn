using Microsoft.Extensions.DependencyInjection;
using Unicorn.Contracts;
using Unicorn.Integration.RabbitMQ;
using Unicorn.Integration.RabbitMQ.Enums;
using Unicorn.Integration.RabbitMQ.Interfaces;
using Unicorn.Tests.RabbitMQ.Consumer.Processors;

namespace Unicorn.Tests.RabbitMQ.Consumer.Services;

public sealed class RabbitNewsDirect : IRabbitAction
{
    private readonly IServiceScopeFactory _scopeFactory;

    public RabbitNewsDirect(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public Task GetResultAsync(QueueEntities entity, QueueActions action, string data)
    {
        var serviceProvider = _scopeFactory.CreateAsyncScope().ServiceProvider;

        return entity switch
        {
            QueueEntities.Trade => serviceProvider
                .GetRequiredService<TradeProcessor>()
                .ProcessAsync(action, JsonHelper.Deserialize<Trade>(data)),
            QueueEntities.Trades => serviceProvider
                .GetRequiredService<TradeProcessor>()
                .ProcessRangeAsync(action, JsonHelper.Deserialize<IEnumerable<Trade>>(data)),
            _ => Task.CompletedTask
        };
    }
}
