using OmniCore.Common.Amqp;

namespace OmniCore.Common.Core;

public interface IAppConfiguration
{
    string? AccountEmail { get; set; }
    bool AccountVerified { get; set; }
    string ClientName { get; set; }
    string ApiAddress { get; set; }
    ClientAuthorization? ClientAuthorization { get; set; }
    AmqpEndpointDefinition? Endpoint { get; set; }
    event EventHandler ConfigurationChanged;
}