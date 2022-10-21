using Unicorn.Integration.RabbitMQ.Enums;
using Unicorn.Integration.RabbitMQ.Interfaces;

namespace Unicorn.Integration.RabbitMQ;

public abstract class RabbitActionBase
{
    private readonly IRabbitConnectionFactory _connectionFactory;
    private readonly Dictionary<QueueExchanges, IRabbitAction> _actions;

    protected RabbitActionBase(IRabbitConnectionFactory connectionFactory, Dictionary<QueueExchanges, IRabbitAction> actions)
    {
        _connectionFactory = connectionFactory;
        _actions = actions;
    }

    public async Task<bool> StartAsync(QueueExchanges exchange, string routingKey, string data)
    {
        var route = routingKey.Split('.');

        try
        {
            if (route.Length < 3)
            {
                throw new ArgumentOutOfRangeException(nameof(route.Length), "Queue route length invalid");
            }

            if (!Enum.TryParse(route[1], true, out QueueEntities entity))
            {
                throw new ArgumentException($"Queue entity '{route[1]}' not recognized");
            }

            if (!Enum.TryParse(route[2], true, out QueueActions action))
            {
                throw new ArgumentException($"Queue action '{route[2]}' not recognized");
            }

            if (!_actions.ContainsKey(exchange))
            {
                throw new ArgumentException($"Exchange '{exchange}' not found");
            }

            await _actions[exchange].GetResultAsync(entity, action, data);

            return true;
        }
        catch (Exception exception)
        {
            // logger.LogError(nameof(StartAsync), exception);
            return false;
        }
    }

    public void Publish<T>(QueueExchanges exchange, QueueNames queue, QueueEntities entity, QueueActions action, T data)
        where T : class
    {
        using var publisher = new RabbitPublisher(_connectionFactory, exchange);
        publisher.Publish(queue, entity, action, data);
    }

    public void Publish<T>(QueueExchanges exchange, IEnumerable<QueueNames> queues, QueueEntities entity, QueueActions action, T data)
        where T : class
    {
        using var publisher = new RabbitPublisher(_connectionFactory, exchange);
        publisher.Publish(queues, entity, action, data);
    }

    public void Publish(QueueExchanges exchange, IEnumerable<(QueueNames, QueueEntities, QueueActions, object)> taskParams)
    {
        using var publisher = new RabbitPublisher(_connectionFactory, exchange);

        foreach (var task in taskParams)
        {
            publisher.Publish(task.Item1,task.Item2, task.Item3, task.Item4);
        }
    }
}
