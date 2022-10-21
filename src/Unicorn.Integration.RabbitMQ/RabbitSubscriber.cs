using System.Text;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Unicorn.Integration.RabbitMQ.Enums;
using Unicorn.Integration.RabbitMQ.Models;

namespace Unicorn.Integration.RabbitMQ;

public sealed class RabbitSubscriber
{
    private readonly ILogger<RabbitSubscriber> _logger;
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly Queue _queue;

    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public RabbitSubscriber(
        ILogger<RabbitSubscriber> logger,
        IRabbitConnectionFactory connectionFactory,
        IEnumerable<QueueExchanges> exchangeNames,
        QueueNames queueName)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(connectionFactory);

        _logger = logger;
        ConnectionFactory = connectionFactory;

        _connection = connectionFactory.CreateConnection();
        _channel = _connection.CreateModel();

        var exchanges = QueueConfiguration.Exchanges.Join(exchangeNames, x => x.NameEnum, y => y, (x, _) => x).ToArray();
        var queues = exchanges.SelectMany(x => x.Queues).Distinct(new QueueComparer());

        _queue = queues.FirstOrDefault(x => x.NameEnum == queueName) ?? throw new NullReferenceException($"{nameof(RabbitSubscriber)}.Error: '{queueName}' was not register");

        foreach (var exchange in exchanges)
        {
            _channel.ExchangeDeclare(exchange.NameString, exchange.Type);

            _channel.QueueDeclare(_queue.NameString, false, false, false, null);

            foreach (var route in _queue.Entities)
            {
                _channel.QueueBind(_queue.NameString, exchange.NameString, $"{_queue.NameString}.{route.NameString}.*");
            }
        }
    }

    internal IRabbitConnectionFactory ConnectionFactory { get; }

    public void Subscribe(Func<QueueExchanges, string, string, Task<bool>> queueFunc)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += OnReceivedAsync;
        _channel.BasicConsume(_queue.NameString, !_queue.WithConfirm, consumer);

        async void OnReceivedAsync(object? _, BasicDeliverEventArgs ea)
        {
            var data = Encoding.UTF8.GetString(ea.Body.ToArray());
            var result = false;

            await _semaphore.WaitAsync();

            try
            {
                var exchange = Enum.Parse<QueueExchanges>(ea.Exchange, true);
                result = await queueFunc(exchange, ea.RoutingKey, data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(RabbitSubscriber)}.{nameof(Subscribe)}");
            }

            _semaphore.Release();

            if (_queue.WithConfirm && result)
            {
                _channel.BasicAck(ea.DeliveryTag, false);
            }
        }
    }
    public void Unsubscribe()
    {
        _channel.Dispose();
        _connection.Dispose();
    }
}
