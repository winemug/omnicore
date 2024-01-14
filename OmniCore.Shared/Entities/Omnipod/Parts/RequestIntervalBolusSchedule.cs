namespace OmniCore.Shared.Entities.Omnipod.Parts;

public class RequestIntervalBolusSchedule : RequestIntervalSchedule, IMessagePart
{
    public int ToBytes(Span<byte> span)
    {
        return ToBytes(span, false);
    }

    public static IMessagePart ToInstance(Span<byte> span)
    {
        var instance = new RequestIntervalBasalSchedule();
        SetInstance(instance, span, false);
        return instance;
    }

}