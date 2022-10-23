using Microsoft.Extensions.Logging;
using Unicorn.Contracts;
using Unicorn.Integration.RabbitMQ.Enums;
using Unicorn.Integration.RabbitMQ.Interfaces;

namespace Unicorn.Tests.RabbitMQ.Consumer.Processors;

public sealed class TradeProcessor : IRabbitProcess
{
    private readonly ILogger<TradeProcessor> _logger;

    public TradeProcessor(ILogger<TradeProcessor> logger)
    {
        _logger = logger;
    }

    private Task<Trade> GetAssets(string action, Trade trade)
    {
        _logger.LogInformation("Action: {Action} | Single trade: {@Trade}", action, trade);
        return Task.FromResult(trade);
    }

    private Task<IEnumerable<Trade>> GetAssetsRange(string action, IEnumerable<Trade> trades)
    {
        _logger.LogInformation("Action: {Action} | Multiple trades: {@Trades}", action, trades);
        return Task.FromResult(trades);
    }

    public Task ProcessAsync<T>(QueueActions action, T model) where T : class
    {
        return model switch
        {
            Trade trade => action switch
            {
                QueueActions.Get => GetAssets("Get", trade),
                QueueActions.Set => GetAssets("Set", trade),
                QueueActions.Persist => GetAssets("Persist", trade),
                _ => Task.CompletedTask
            },
            _ => Task.CompletedTask
        };
    }

    public Task ProcessRangeAsync<T>(QueueActions action, IEnumerable<T> models) where T : class
    {
        return models switch
        {
            IEnumerable<Trade> trades => action switch
            {
                QueueActions.Get => GetAssetsRange("Get", trades),
                QueueActions.Set => GetAssetsRange("Set", trades),
                QueueActions.Persist => GetAssetsRange("Persist", trades),
                _ => Task.CompletedTask
            },
            _ => Task.CompletedTask
        };
    }
}
