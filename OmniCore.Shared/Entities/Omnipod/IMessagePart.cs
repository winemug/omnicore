using OmniCore.Shared.Enums;

namespace OmniCore.Shared.Entities.Omnipod;

public interface IMessagePart
{
    bool RequiresNonce { get; }
    PodMessagePartType MessagePartType { get; }
    int ToBytes(Span<byte> span);
    static abstract IMessagePart ToInstance(Span<byte> span);
}
