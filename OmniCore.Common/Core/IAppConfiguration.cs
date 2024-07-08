namespace OmniCore.Services.Interfaces.Core;

public interface IAppConfiguration
{
    public Task Set(OmniCoreConfiguration configuration);
    public Task<OmniCoreConfiguration?> Get();
}

public abstract record OmniCoreConfiguration
{ 
    public Guid ClientId { get; } = Guid.Empty;
    public Guid AccountId { get; } = Guid.Empty;
    public Guid DefaultProfileId { get; } = Guid.Empty;
    public string AmqpConnectionString { get; } = "";
    public string ClientCertificate { get; } = "";
    public string ClientKey { get; } = "";

    public string UserId { get; } = "";
    public string RequestExchange { get; } = "";
    public string SyncExchange { get; } = "";
    public string ResponseExchange { get; } = "";
}
