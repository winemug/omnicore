using System;
using System.Threading;
using System.Threading.Tasks;
using OmniCore.Services.Interfaces.Definitions;
using OmniCore.Services.Interfaces.Entities;

namespace OmniCore.Services.Interfaces;

public interface IPod
{
    Guid Id { get; set; }
    uint RadioAddress { get; set; }
    int UnitsPerMilliliter { get; set; }
    MedicationType Medication { get; set; }
    DateTimeOffset ValidFrom { get; set; }
    DateTimeOffset? ValidTo { get; set; }
    PodRuntimeInformation Info { get; set; }
    Task<IDisposable> LockAsync(CancellationToken cancellationToken);
    Task ProcessResponseAsync(IPodMessage message);
    uint NextNonce();
    void SyncNonce(ushort syncWord, int syncMessageSequence);
}