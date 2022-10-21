using Unicorn.Integration.RabbitMQ.Enums;

namespace Unicorn.Integration.RabbitMQ.Interfaces;

public interface IRabbitProcess
{
    Task ProcessAsync<T>(QueueActions action, T model) where T : class;
    Task ProcessRangeAsync<T>(QueueActions action, IEnumerable<T> models) where T : class;
}
