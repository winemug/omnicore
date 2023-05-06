using System;
using System.Threading;
using System.Threading.Tasks;

namespace OmniCore.Services.Interfaces.Pod;

public interface IPodConnection : IDisposable
{
    Task<PodRequestStatus> PrimePodAsync(DateOnly podDate, TimeOnly podTime, bool relaxDeliveryCrosschecks, CancellationToken cancellationToken);
    Task<PodRequestStatus> StartPodAsync(TimeOnly podTime, BasalRateEntry[] basalRateEntries, CancellationToken cancellationToken = default);
    Task<PodRequestStatus> ConfigureAlerts(
        AlertConfiguration[] alertConfigurations,
        CancellationToken cancellationToken = default);
    Task<PodRequestStatus> UpdateStatus(CancellationToken cancellationToken = default);
    Task<PodRequestStatus> Beep(BeepType type, CancellationToken cancellationToken = default);
    Task<PodRequestStatus> SetBasalSchedule(
        TimeOnly podTime,
        int[] pulsesPerHour48HalfHours,
        CancellationToken cancellationToken = default);
    Task<PodRequestStatus> SetTempBasal(
        int hourlyRateMilliunits,
        int halfHourCount,
        CancellationToken cancellationToken = default);
    Task<PodRequestStatus> CancelTempBasal(CancellationToken cancellationToken = default);
    Task<PodRequestStatus> Bolus(
        int bolusPulses,
        int pulseIntervalSeconds,
        CancellationToken cancellationToken = default);
    Task<PodRequestStatus> CancelBolus(CancellationToken cancellationToken = default);
    Task<PodRequestStatus> Deactivate(CancellationToken cancellationToken = default);
    Task<PodRequestStatus> AcknowledgeAlerts(CancellationToken cancellationToken = default);
}
