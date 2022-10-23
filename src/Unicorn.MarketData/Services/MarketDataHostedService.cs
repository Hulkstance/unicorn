using Binance.Net.Interfaces.Clients;
using Binance.Net.Objects.Models.Spot.Socket;
using CryptoExchange.Net.Sockets;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Unicorn.Algo.Indicators;
using Unicorn.Contracts;
using Unicorn.Integration.RabbitMQ;
using Unicorn.Integration.RabbitMQ.Enums;
using Unicorn.Integration.RabbitMQ.Interfaces;

namespace Unicorn.MarketData.Services;

internal sealed class MarketDataHostedService : BackgroundService
{
    private readonly ILogger<MarketDataHostedService> _logger;
    private readonly IBinanceSocketClient _socketClient;
    private readonly RabbitPublisher _publisher;
    private readonly Dictionary<string, VolumeSpike> _symbols = new();

    private UpdateSubscription? _updateSubscription;

    public MarketDataHostedService(
        ILogger<MarketDataHostedService> logger,
        IBinanceSocketClient socketClient,
        IRabbitConnectionFactory connectionFactory)
    {
        _logger = logger;
        _socketClient = socketClient;
        _publisher = new RabbitPublisher(connectionFactory, QueueExchanges.NewsDirect);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var symbols = new[] { "BNBUSDT", "ADAUSDT" };

        foreach (var symbol in symbols)
        {
            const int period = 20;
            const int multiplier = 2;
            _symbols[symbol] = new VolumeSpike(period, multiplier);
        }

        var subResult = await _socketClient.SpotStreams.SubscribeToTradeUpdatesAsync(symbols, data =>
        {
            var symbol = data.Data.Symbol;
            Print(data);
            VolumeSpikeProcessor(symbol, data);
        }, stoppingToken);

        if (!subResult)
        {
            _logger.LogError("Error while subscribing to the trade endpoint. {Error}", subResult.Error);
            return;
        }

        if (stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Cancellation requested");
            await subResult.Data.CloseAsync();
            return;
        }

        _updateSubscription = subResult.Data;

        void Print(DataEvent<BinanceStreamTrade> data)
        {
            _logger.LogInformation("{Timestamp:HH:mm:ss.fff}, {Symbol}, {Price}, {Volume}",
                data.Timestamp, data.Data.Symbol, data.Data.Price, data.Data.Quantity);
        }

        void VolumeSpikeProcessor(string symbol, DataEvent<BinanceStreamTrade> data)
        {
            if (!_symbols.TryGetValue(symbol, out var volumeSpike))
            {
                return;
            }

            var trade = new Trade(
                data.Timestamp,
                data.Data.Symbol,
                data.Data.Price,
                data.Data.Quantity);

            var isVolumeSpikeTriggered = volumeSpike.ComputeNextValue(trade.Quantity);

            if (!isVolumeSpikeTriggered.HasValue || !isVolumeSpikeTriggered.Value)
            {
                return;
            }

            Console.WriteLine($"{symbol} | Volume Spike triggered");

            _publisher.Publish(
                QueueNames.Signals,
                QueueEntities.Trade,
                QueueActions.Persist,
                trade with { IsVolumeSpikeTriggered = isVolumeSpikeTriggered });
        }
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
