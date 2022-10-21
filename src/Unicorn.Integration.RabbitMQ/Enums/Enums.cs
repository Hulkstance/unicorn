namespace Unicorn.Integration.RabbitMQ.Enums;

public enum QueueNames
{
    Signals
}

public enum QueueExchanges
{
    NewsDirect
}

public enum QueueEntities
{
    Ticker,
    Tickers
}

public enum QueueActions
{
    Get,
    Set,
    Compute
}
