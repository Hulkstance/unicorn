using System.Net.Security;
using System.Security.Authentication;
using RabbitMQ.Client;

namespace Unicorn.Integration.RabbitMQ;

public sealed record RabbitSslOptions
{
    public bool Enabled { get; init; }
    public SslProtocols Protocols { get; init; } = SslProtocols.Tls12;
    public SslPolicyErrors AcceptablePolicyErrors { get; init; } = SslPolicyErrors.None;
    public RemoteCertificateValidationCallback? CertificateValidationCallback { get; set; }
}

public abstract record RabbitBaseOptions
{
    public string Host { get; init; } = string.Empty;

    public string VirtualHost { get; init; } = string.Empty;

    public string User { get; init; } = string.Empty;

    public string Password { get; init; } = string.Empty;

    public int Port { get; init; } = AmqpTcpEndpoint.UseDefaultPort;
    public TimeSpan RequestedHeartbeat { get; set; } = TimeSpan.FromSeconds(60);
    public RabbitSslOptions Tls { get; init; } = new();
}

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

public sealed class RabbitConnectionFactory : IRabbitConnectionFactory
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly Lazy<IConnection> _lazyConnection;
    private readonly Lazy<IModel> _lazyChannel;

    public RabbitConnectionFactory(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;

        _lazyConnection = new Lazy<IConnection>(() =>
            _connectionFactory.CreateConnection(), LazyThreadSafetyMode.ExecutionAndPublication);

        _lazyChannel = new Lazy<IModel>(() =>
            CurrentConnection.CreateModel(), LazyThreadSafetyMode.ExecutionAndPublication);
    }

    public static IRabbitConnectionFactory From<TOptions>(TOptions options) where TOptions : RabbitBaseOptions
    {
        ArgumentNullException.ThrowIfNull(options);
        return new RabbitConnectionFactory(FromConfig(options));
    }

    private static IConnectionFactory FromConfig(RabbitBaseOptions baseOptions)
    {
        ArgumentNullException.ThrowIfNull(baseOptions);

        return new ConnectionFactory
        {
            HostName = baseOptions.Host,
            Port = baseOptions.Port,
            VirtualHost = baseOptions.VirtualHost,
            UserName = baseOptions.User,
            Password = baseOptions.Password,
            RequestedHeartbeat = baseOptions.RequestedHeartbeat,
            Ssl = new SslOption
            {
                Enabled = baseOptions.Tls.Enabled,
                ServerName = baseOptions.Host,
                Version = baseOptions.Tls.Enabled ? baseOptions.Tls.Protocols : SslProtocols.None,
                AcceptablePolicyErrors = baseOptions.Tls.AcceptablePolicyErrors,
                CertificateValidationCallback = baseOptions.Tls.CertificateValidationCallback
            }
        };
    }

    public string UserName => _connectionFactory.UserName;
    public string Password => _connectionFactory.Password;
    public string VirtualHost => _connectionFactory.VirtualHost;

    public IConnection CurrentConnection => _lazyConnection.Value;

    public IModel CurrentChannel => _lazyChannel.Value;

    public IConnection CreateConnection() => _connectionFactory.CreateConnection();

    public IModel CreateModel() => CurrentConnection.CreateModel();

    public void Dispose()
    {
        if (_lazyChannel.IsValueCreated)
        {
            _lazyChannel.Value.Dispose();
        }

        if (_lazyConnection.IsValueCreated)
        {
            _lazyConnection.Value.Dispose();
        }
    }
}
