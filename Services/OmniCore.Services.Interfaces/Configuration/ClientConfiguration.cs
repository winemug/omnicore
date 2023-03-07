using System;
using OmniCore.Services.Interfaces.Cgm;

namespace OmniCore.Services.Interfaces.Configuration;

public class ClientConfiguration
{
    public Guid? AccountId { get; set; }
    public Guid? ClientId { get; set; }
    public string ClientAuthorizationToken { get; set; }
    public string Name { get; set; }

    public Guid? DefaultProfileId { get; set; }
    public CgmReceiverType? ReceiverType { get; set; }
    public Guid? ReceiverProfileId { get; set; }
}