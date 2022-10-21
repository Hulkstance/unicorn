using Microsoft.Extensions.Logging;
using Unicorn.Contracts;
using Unicorn.Integration.RabbitMQ.Enums;
using Unicorn.Integration.RabbitMQ.Interfaces;

namespace Unicorn.Tests.RabbitMQ.Consumer.Processors;

public sealed class TickerProcessor : IRabbitProcess
{
    private readonly ILogger<TickerProcessor> _logger;

    public TickerProcessor(ILogger<TickerProcessor> logger)
    {
        _logger = logger;
    }

    private Task<Ticker> GetAssets(string action, Ticker ticker)
    {
        _logger.LogInformation("Action: {Action} | Single Dto: {@Dto}", action, ticker);
        return Task.FromResult(ticker);
    }

    private Task<IEnumerable<Ticker>> GetAssetsRange(string action, IEnumerable<Ticker> ticker)
    {
        _logger.LogInformation("Action: {Action} | Multiple Dtos: {@Dto}", action, ticker);
        return Task.FromResult(ticker);
    }

    public Task ProcessAsync<T>(QueueActions action, T model) where T : class
    {
        return model switch
        {
            Ticker ticker => action switch
            {
                QueueActions.Get => GetAssets("Get", ticker),
                QueueActions.Set => GetAssets("Set", ticker),
                QueueActions.Compute => GetAssets("Compute", ticker),
                _ => Task.CompletedTask
            },
            _ => Task.CompletedTask
        };
    }

    public Task ProcessRangeAsync<T>(QueueActions action, IEnumerable<T> models) where T : class
    {
        return models switch
        {
            IEnumerable<Ticker> tickers => action switch
            {
                QueueActions.Get => GetAssetsRange("Get", tickers),
                QueueActions.Set => GetAssetsRange("Set", tickers),
                QueueActions.Compute => GetAssetsRange("Compute", tickers),
                _ => Task.CompletedTask
            },
            _ => Task.CompletedTask
        };
    }
}
