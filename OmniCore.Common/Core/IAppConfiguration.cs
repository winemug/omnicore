namespace OmniCore.Services.Interfaces.Core;

public interface IAppConfiguration
{
    public Task Set(OmniCoreConfiguration configuration);
    public Task<OmniCoreConfiguration?> Get();
}

public record OmniCoreConfiguration
{ 
    public Guid ClientId { get; set; } = Guid.Empty;
    public Guid AccountId { get; set;} = Guid.Empty;
    public Guid DefaultProfileId { get; set;} = Guid.Empty;
    public string AmqpConnectionString { get; set;} = "";
    public string ClientCertificate { get; set;} = "";
    public string ClientKey { get; set;} = "";

    public string UserId { get; set;} = "";
    public string RequestExchange { get; set;} = "";
    public string SyncExchange { get; set;} = "";
    public string ResponseExchange { get; set;} = "";
}
