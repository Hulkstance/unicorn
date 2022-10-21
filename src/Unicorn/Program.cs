using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Unicorn.MarketData;
using Unicorn.Logging;

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

var host = new HostBuilder()
    .ConfigureAppConfiguration(c => c
        .AddEnvironmentVariables()
        .AddJsonFile($"appsettings.{environment}.json"))
    .ConfigureServices((context, services) =>
    {
        services.AddMarketData(context.Configuration);
    })
    .ConfigureLogging()
    .UseConsoleLifetime()
    .Build();

await host.RunAsync();
