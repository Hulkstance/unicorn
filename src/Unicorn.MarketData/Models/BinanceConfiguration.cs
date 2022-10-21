namespace Unicorn.MarketData.Models;

public sealed class BinanceConfiguration
{
    public string ApiKey { get; init; } = default!;
    public string SecretKey { get; init; } = default!;
}
