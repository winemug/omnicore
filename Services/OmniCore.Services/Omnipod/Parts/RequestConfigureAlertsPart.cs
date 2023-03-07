using OmniCore.Services.Interfaces.Definitions;

namespace OmniCore.Services;

public class RequestConfigureAlertsPart : MessagePart
{
    public override bool RequiresNonce => true;
    public override PodMessageType Type => PodMessageType.RequestConfigureAlerts;
}