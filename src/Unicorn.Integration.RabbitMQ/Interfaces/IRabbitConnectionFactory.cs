using RabbitMQ.Client;

namespace Unicorn.Integration.RabbitMQ.Interfaces;

public interface IRabbitConnectionFactory : IDisposable
{
    /// <summary>
    /// Username to use when authenticating to the server.
    /// </summary>
    string UserName { get; }

    /// <summary>
    /// Password to use when authenticating to the server.
    /// </summary>
    string Password { get; }

    /// <summary>
    /// Virtual host to access during this connection.
    /// </summary>
    string VirtualHost { get; }

    IConnection CurrentConnection { get; }
    IModel CurrentChannel { get; }
    IConnection CreateConnection();
    IModel CreateModel();
}
