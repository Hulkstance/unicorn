using Unicorn.Integration.RabbitMQ.Enums;

namespace Unicorn.Integration.RabbitMQ.Models;

public sealed class QueueEntity
{
    public QueueEntity(QueueEntities entity)
    {
        NameString = entity.ToString();
        NameEnum = entity;
    }
    public string NameString { get; }
    public QueueEntities NameEnum { get; }
    public QueueActions[] Actions { get; init; } = Array.Empty<QueueActions>();
}
