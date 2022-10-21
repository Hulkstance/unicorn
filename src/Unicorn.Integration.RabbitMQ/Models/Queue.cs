using Unicorn.Integration.RabbitMQ.Enums;

namespace Unicorn.Integration.RabbitMQ.Models;

public sealed class Queue
{
    public Queue(QueueNames name, bool withConfirm = false)
    {
        NameString = name.ToString();
        NameEnum = name;
        WithConfirm = withConfirm;
    }

    public string NameString { get; }
    public QueueNames NameEnum { get; }
    public bool WithConfirm { get; }
    public QueueEntity[] Entities { get; init; } = Array.Empty<QueueEntity>();
}
