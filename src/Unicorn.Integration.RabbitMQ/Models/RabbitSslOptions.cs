using System.Net.Security;
using System.Security.Authentication;

namespace Unicorn.Integration.RabbitMQ.Models;

public record RabbitSslOptions
{
    public bool Enabled { get; init; }
    public SslProtocols Protocols { get; init; } = SslProtocols.Tls12;
    public SslPolicyErrors AcceptablePolicyErrors { get; init; } = SslPolicyErrors.None;
    public RemoteCertificateValidationCallback? CertificateValidationCallback { get; set; }
}
