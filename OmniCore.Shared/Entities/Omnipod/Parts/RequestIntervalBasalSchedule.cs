namespace OmniCore.Shared.Entities.Omnipod.Parts;

public class RequestIntervalBasalSchedule : RequestIntervalSchedule, IMessagePart
{
    public int ToBytes(Span<byte> span)
    {
        return ToBytes(span, true);
    }

    public static IMessagePart ToInstance(Span<byte> span)
    {
        var instance = new RequestIntervalBasalSchedule();
        SetInstance(instance, span, true);
        return instance;
    }
}