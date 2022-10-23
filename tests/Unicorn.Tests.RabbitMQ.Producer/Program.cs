using RabbitMQ.Client;
using Unicorn.Contracts;
using Unicorn.Integration.RabbitMQ;
using Unicorn.Integration.RabbitMQ.Enums;

var factory = new ConnectionFactory
{
    HostName = "localhost"
};
var publisher = new RabbitPublisher(new RabbitConnectionFactory(factory), QueueExchanges.NewsDirect);

var trades = new List<Trade>
{
    new(DateTimeOffset.Now, "BTCUSDT",530, 53456, true),
    new(DateTimeOffset.Now, "BTCUSDT",213, 111156, false),
};
publisher.Publish(QueueNames.Signals, QueueEntities.Trades, QueueActions.Trigger, trades);
