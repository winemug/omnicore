namespace OmniCore.Services.Interfaces.Amqp;

public record AmqpEndpointDefinition
{
    public string UserId { get; init; }
    public string Dsn { get; init; }
    public string RequestExchange { get; init; }
    public string ResponseExchange { get; init; }
    public string SyncExchange { get; init; }
}