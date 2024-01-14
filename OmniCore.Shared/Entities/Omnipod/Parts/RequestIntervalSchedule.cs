using OmniCore.Shared.Enums;
using OmniCore.Shared.Extensions;

namespace OmniCore.Shared.Entities.Omnipod.Parts;

public class RequestIntervalSchedule
{
    public bool RequiresNonce => false;
    public PodMessagePartType MessagePartType => PodMessagePartType.RequestInsulinSchedule;
    public ushort LeadPulses10Count { get; set; }
    public uint LeadPulse10DelayMicroseconds { get; set; }
    public byte ActiveIndex { get; set; }
    public PulseInterval[] Pulse10Intervals { get; set; }
    public bool BeepWhenSet { get; set; }
    public bool BeepWhenFinished { get; set; }

    protected static void SetInstance(RequestIntervalSchedule instance, Span<byte> span, bool withIndex)
    {
        if (withIndex)
        {
            instance.BeepWhenSet = span[0] == 0x80;
            instance.ActiveIndex = span[1];
            instance.LeadPulses10Count = span[2..].Read16();
            instance.LeadPulse10DelayMicroseconds = span[4..].Read32();
            instance.Pulse10Intervals = GetPulseIntervals(span[8..]);
        }
        else
        {
            instance.BeepWhenSet = span[0] == 0x80;
            instance.LeadPulses10Count = span[1..].Read16();
            instance.LeadPulse10DelayMicroseconds = span[3..].Read32();
            instance.Pulse10Intervals = GetPulseIntervals(span[7..]);
        }
    }
    protected int ToBytes(Span<byte> span, bool withIndex)
    {
        int idx = 0;
        if (BeepWhenSet)
            span[idx] |= 0x80;
        if (BeepWhenFinished)
            span[idx] |= 0x40;
        idx++;
        if (withIndex)
            span[idx++] = ActiveIndex;
        span[idx..].Write16(LeadPulses10Count);
        idx += 2;
        span[idx..].Write32(LeadPulse10DelayMicroseconds);

        idx += 4;
        foreach (var interval in Pulse10Intervals)
        {
            span[idx..].Write16(interval.Pulse10Count);
            idx += 2;
            span[idx..].Write32(interval.IntervalMicroseconds);
            idx += 4;
        }
        return idx;
    }

    private static PulseInterval[] GetPulseIntervals(Span<byte> span)
    {
        var intervals = new List<PulseInterval>();
        while (span.Length > 0)
        {
            intervals.Add(new PulseInterval
            {
                Pulse10Count = span[0..].Read16(),
                IntervalMicroseconds = span[2..].Read32()
            });
            span = span[6..];
        }
        return intervals.ToArray();
    }


}