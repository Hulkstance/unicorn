namespace Unicorn.Contracts;

public record Ticker(DateTimeOffset Timestamp, string Symbol, decimal Price, decimal Volume);

