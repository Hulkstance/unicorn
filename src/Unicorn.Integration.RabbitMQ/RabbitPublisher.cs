using System.Text;
using RabbitMQ.Client;
using Unicorn.Integration.RabbitMQ.Enums;
using Unicorn.Integration.RabbitMQ.Interfaces;
using Unicorn.Integration.RabbitMQ.Models;

namespace Unicorn.Integration.RabbitMQ;

public sealed class RabbitPublisher : IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly QueueExchange _exchange;

    public RabbitPublisher(IRabbitConnectionFactory connectionFactory, QueueExchanges exchangeName)
    {
        ArgumentNullException.ThrowIfNull(connectionFactory);

        ConnectionFactory = connectionFactory;

        _connection = connectionFactory.CreateConnection();
        _channel = _connection.CreateModel();

        var currentExchange = QueueConfiguration.Exchanges.FirstOrDefault(x => x.NameEnum == exchangeName);
        _exchange = currentExchange ?? throw new NullReferenceException(exchangeName.ToString());

        _channel.ExchangeDeclare(currentExchange.NameString, currentExchange.Type);

        foreach (var queue in currentExchange.Queues.Distinct(new QueueComparer()))
        {
            _channel.QueueDeclare(queue.NameString, false, false, false, null);

            foreach (var route in queue.Entities)
            {
                _channel.QueueBind(queue.NameString, currentExchange.NameString,
                    $"{queue.NameString}.{route.NameString}.*");
            }
        }
    }

    internal IRabbitConnectionFactory ConnectionFactory { get; }

    /// <inheritdoc />
    public void Dispose()
    {
        _connection.Dispose();
        _channel.Dispose();
        ConnectionFactory.Dispose();
    }

    public void Publish(QueueNames queue, QueueEntities entity, QueueActions action, object data)
    {
        var json = JsonHelper.Serialize(data);

        var currentQueue = _exchange.Queues.FirstOrDefault(x => x.NameEnum == queue);

        if (currentQueue is null)
        {
            return;
        }

        var queueEntity = currentQueue.Entities.FirstOrDefault(x => x.NameEnum == entity && x.Actions.Contains(action));

        if (queueEntity is null)
        {
            // $"Entity '{entity}' for Action '{action}' from Queue '{queue}' by Exchange '{_exchange.NameString}' not found!");
            return;
        }

        _channel.BasicPublish(
            _exchange.NameString,
            $"{currentQueue.NameString}.{queueEntity.NameString}.{action.ToString()}",
            null,
            Encoding.UTF8.GetBytes(json));
    }

    public void Publish<T>(QueueNames queue, QueueEntities entity, QueueActions action, T data)
        where T : class
    {
        var json = JsonHelper.Serialize(data);

        var currentQueue = _exchange.Queues.FirstOrDefault(x => x.NameEnum == queue);

        if (currentQueue is null)
        {
            return;
        }

        var queueEntity = currentQueue.Entities.FirstOrDefault(x => x.NameEnum == entity && x.Actions.Contains(action));

        if (queueEntity is null)
        {
            // $"Entity '{entity}' for Action '{action}' from Queue '{queue}' by Exchange '{_exchange.NameString}' not found!");
            return;
        }

        _channel.BasicPublish(
            _exchange.NameString,
            $"{currentQueue.NameString}.{queueEntity.NameString}.{action.ToString()}",
            null,
            Encoding.UTF8.GetBytes(json));
    }

    public void Publish<T>(IEnumerable<QueueNames> queues, QueueEntities entity, QueueActions action, T data)
        where T : class
    {
        var json = JsonHelper.Serialize(data);

        foreach (var queue in _exchange.Queues.Join(queues, x => x.NameEnum, y => y, (x, _) => x))
        foreach (var queueEntity in queue.Entities.Where(x => x.NameEnum == entity && x.Actions.Contains(action)))
        {
            _channel.BasicPublish(
                _exchange.NameString,
                $"{queue.NameString}.{queueEntity.NameString}.{action.ToString()}",
                null,
                Encoding.UTF8.GetBytes(json));
        }
    }
}
