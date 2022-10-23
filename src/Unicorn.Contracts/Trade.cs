namespace Unicorn.Contracts;

public record Trade(DateTimeOffset Timestamp, string Symbol, decimal Price, decimal Volume);
