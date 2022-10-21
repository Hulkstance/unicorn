using Unicorn.Integration.RabbitMQ.Enums;
using Unicorn.Integration.RabbitMQ.Models;

namespace Unicorn.Integration.RabbitMQ;

public static class QueueConfiguration
{
    public static IEnumerable<QueueExchange> Exchanges => new[]
    {
        new QueueExchange(QueueExchanges.NewsDirect)
        {
            Queues = new[]
            {
                new Queue(QueueNames.Signals)
                {
                    Entities = new[]
                    {
                        new QueueEntity(QueueEntities.Ticker)
                        {
                            Actions = new[]
                            {
                                QueueActions.Get,
                                QueueActions.Set,
                                QueueActions.Compute
                            }
                        },
                        new QueueEntity(QueueEntities.Tickers)
                        {
                            Actions = new[]
                            {
                                QueueActions.Get,
                                QueueActions.Set,
                                QueueActions.Compute
                            }
                        }
                    }
                }
            }
        }
    };
}
