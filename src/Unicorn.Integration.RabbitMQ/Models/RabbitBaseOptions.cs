using RabbitMQ.Client;

namespace Unicorn.Integration.RabbitMQ.Models;

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
