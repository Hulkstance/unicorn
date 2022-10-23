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
                        new QueueEntity(QueueEntities.Trade)
                        {
                            Actions = new[]
                            {
                                QueueActions.Get,
                                QueueActions.Set,
                                QueueActions.Persist
                            }
                        },
                        new QueueEntity(QueueEntities.Trades)
                        {
                            Actions = new[]
                            {
                                QueueActions.Get,
                                QueueActions.Set,
                                QueueActions.Persist
                            }
                        }
                    }
                }
            }
        }
    };
}
