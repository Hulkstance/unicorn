using Unicorn.Integration.RabbitMQ.Enums;

namespace Unicorn.Integration.RabbitMQ.Interfaces;

public interface IRabbitAction
{
    Task GetResultAsync(QueueEntities entity, QueueActions action, string data);
}
