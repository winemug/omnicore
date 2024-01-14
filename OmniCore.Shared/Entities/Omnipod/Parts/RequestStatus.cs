using OmniCore.Shared.Enums;

namespace OmniCore.Shared.Entities.Omnipod.Parts;

public class RequestStatus : IMessagePart
{
    public bool RequiresNonce => false;
    public PodMessagePartType MessagePartType => PodMessagePartType.RequestStatus;
    public required PodStatusType StatusType { get; set; }
    public static IMessagePart ToInstance(Span<byte> span)
    {
        return new RequestStatus
        {
            StatusType = (PodStatusType)span[0]
        };
    }

    public int ToBytes(Span<byte> span)
    {
        span[0] = (byte)StatusType;
        return 1;
    }
}