using RabbitMQ.Client;
using Unicorn.Contracts;
using Unicorn.Integration.RabbitMQ;
using Unicorn.Integration.RabbitMQ.Enums;

var factory = new ConnectionFactory
{
    HostName = "localhost"
};
var publisher = new RabbitPublisher(new RabbitConnectionFactory(factory), QueueExchanges.NewsDirect);

var tickers = new List<Ticker>
{
    new(DateTimeOffset.Now, "BTCUSDT",530, 53456),
    new(DateTimeOffset.Now, "BTCUSDT",213, 111156),
};
publisher.Publish(QueueNames.Signals, QueueEntities.Tickers, QueueActions.Compute, tickers);
