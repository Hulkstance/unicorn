using Microsoft.Extensions.DependencyInjection;
using Unicorn.Integration.RabbitMQ;
using Unicorn.Integration.RabbitMQ.Enums;
using Unicorn.Integration.RabbitMQ.Interfaces;

namespace Unicorn.Tests.RabbitMQ.Consumer.Services;

public class RabbitAction : RabbitActionBase
{
    public RabbitAction(IRabbitConnectionFactory connectionFactory, IServiceScopeFactory scopeFactory)
        : base(connectionFactory,
            new Dictionary<QueueExchanges, IRabbitAction>
            {
                { QueueExchanges.NewsDirect, new RabbitNewsDirect(scopeFactory) }
            })
    {
    }
}
