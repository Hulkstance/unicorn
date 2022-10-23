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
    Trade,
    Trades
}

public enum QueueActions
{
    Get,
    Set,
    Persist
}
