using Binance.Net;
using CryptoExchange.Net.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using Unicorn.Integration.RabbitMQ;
using Unicorn.Integration.RabbitMQ.Interfaces;
using Unicorn.MarketData.Models;
using Unicorn.MarketData.Services;

namespace Unicorn.MarketData;

public static class MarketDataExtensions
{
    public static IServiceCollection AddMarketData(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddBinance((restClientOptions, socketClientOptions) =>
        {
            var binanceConfig = configuration.GetSection("ExchangeOptions:Binance").Get<BinanceConfiguration>();

            restClientOptions.ApiCredentials = new ApiCredentials(binanceConfig.ApiKey, binanceConfig.SecretKey);
            restClientOptions.SpotApiOptions.AutoTimestamp = true;
            restClientOptions.SpotApiOptions.TimestampRecalculationInterval = TimeSpan.FromMinutes(30);

            socketClientOptions.ApiCredentials = new ApiCredentials(binanceConfig.ApiKey, binanceConfig.SecretKey);
            socketClientOptions.AutoReconnect = true;
            socketClientOptions.ReconnectInterval = TimeSpan.FromSeconds(15);
        }, ServiceLifetime.Singleton);

        services.AddHostedService<MarketDataHostedService>();

        // RabbitMQ
        var factory = new ConnectionFactory
        {
            HostName = configuration.GetValue<string>("RabbitMQOptions:Host")
        };
        services.AddSingleton<IRabbitConnectionFactory>(new RabbitConnectionFactory(factory));

        return services;
    }
}
