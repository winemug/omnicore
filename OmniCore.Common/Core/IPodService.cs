
using OmniCore.Services.Interfaces.Pod;

namespace OmniCore.Services.Interfaces.Core;

public interface IPodService : ICoreService
{
    Task Refresh();
    Task<Guid> NewPodAsync(
        int unitsPerMilliliter,
        MedicationType medicationType,
        uint? radioAddress);

    Task RemovePodAsync(Guid podId, DateTimeOffset? removeTime = null);
    Task<Guid> ImportPodAsync(
        uint radioAddress, int unitsPerMilliliter,
        MedicationType medicationType,
        uint Lot,
        uint Serial);
    Task<List<IPodModel>> GetPodsAsync();
    Task<IPodModel?> GetPodAsync(Guid podId);

    Task<IPodConnection> GetConnectionAsync(
        IPodModel podModel,
        CancellationToken cancellationToken = default);
}
