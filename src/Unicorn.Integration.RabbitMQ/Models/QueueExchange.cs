using RabbitMQ.Client;
using Unicorn.Integration.RabbitMQ.Enums;

namespace Unicorn.Integration.RabbitMQ.Models;

public sealed class QueueExchange
{
    public QueueExchange(QueueExchanges name, string type = ExchangeType.Topic)
    {
        NameString = name.ToString();
        Type = string.IsNullOrWhiteSpace(type) ? ExchangeType.Topic : type;
        NameEnum = name;
    }

    public string NameString { get; }
    public QueueExchanges NameEnum { get; }
    public string Type { get; }

    public Queue[] Queues { get; init; } = Array.Empty<Queue>();
}
