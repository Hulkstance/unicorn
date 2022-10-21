using Binance.Net.Interfaces.Clients;
using CryptoExchange.Net.Sockets;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Unicorn.Contracts;
using Unicorn.Integration.RabbitMQ;
using Unicorn.Integration.RabbitMQ.Enums;

namespace Unicorn.MarketData.Services;

internal sealed class MarketDataHostedService : BackgroundService
{
    private readonly ILogger<MarketDataHostedService> _logger;
    private readonly IBinanceSocketClient _socketClient;
    private readonly IRabbitConnectionFactory _connectionFactory;

    private UpdateSubscription? _updateSubscription;

    public MarketDataHostedService(
        ILogger<MarketDataHostedService> logger,
        IBinanceSocketClient socketClient,
        IRabbitConnectionFactory connectionFactory)
    {
        _logger = logger;
        _socketClient = socketClient;
        _connectionFactory = connectionFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var publisher = new RabbitPublisher(_connectionFactory, QueueExchanges.NewsDirect);
        var symbols = new[] { "BNBUSDT", "ADAUSDT" };

        var subResult = await _socketClient.SpotStreams.SubscribeToTickerUpdatesAsync(symbols, data =>
        {
            _logger.LogInformation("{Timestamp:HH:mm:ss.fff}, {Symbol}, {Price}, {Volume}",
                data.Timestamp, data.Data.Symbol, data.Data.LastPrice, data.Data.Volume);

            var ticker = new Ticker(data.Timestamp, data.Data.Symbol, data.Data.LastPrice, data.Data.Volume);
            publisher.Publish(QueueNames.Signals, QueueEntities.Ticker, QueueActions.Compute, ticker);
        }, stoppingToken);

        if (!subResult)
        {
            _logger.LogError("Error while subscribing to the ticker endpoint. {Error}", subResult.Error);
            return;
        }

        if (stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Cancellation requested");
            await subResult.Data.CloseAsync();
            return;
        }

        _updateSubscription = subResult.Data;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_updateSubscription is not null)
        {
            await _updateSubscription.CloseAsync();
        }

        await base.StopAsync(cancellationToken);
    }
}
