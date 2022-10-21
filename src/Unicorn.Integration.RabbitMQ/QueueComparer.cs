using Unicorn.Integration.RabbitMQ.Models;

namespace Unicorn.Integration.RabbitMQ;

internal sealed class QueueComparer : IEqualityComparer<Queue>
{
    public bool Equals(Queue? x, Queue? y)
    {
        return x!.NameEnum == y!.NameEnum;
    }

    public int GetHashCode(Queue obj)
    {
        return obj.NameString.GetHashCode();
    }
}
