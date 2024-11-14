using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Nito.AsyncEx;
using OmniCore.Common.Data;
using OmniCore.Services.Interfaces;
using OmniCore.Services.Interfaces.Core;
using OmniCore.Services.Interfaces.Pod;
using Plugin.BLE.Abstractions;


namespace OmniCore.Services;

public class PodService : IPodService
{
    private IRadioService _radioService;
    private ISyncService _syncService;
    private IAppConfiguration _appConfiguration;
    
    private ConcurrentDictionary<Guid, AsyncLock> _podLocks = new ConcurrentDictionary<Guid, AsyncLock>();
    private List<IPodModel> _podModels = new List<IPodModel>();

    public PodService(
        IRadioService radioService,
        IAppConfiguration appConfiguration,
        ISyncService syncService)
    {
        _radioService = radioService;
        _appConfiguration = appConfiguration;
        _syncService = syncService;
    }

    public async Task Start()
    {
        try
        {
            using var ocdb = new OcdbContext();
            //var removedPods = ocdb.Pods.Where(p => p.Removed.HasValue && p.IsSynced).ToList();
            //foreach(var removedPod in removedPods)
            //{
            //    if (ocdb.PodActions.Any(pa => pa.PodId == removedPod.PodId && !pa.IsSynced))
            //        continue;
            //    Debug.WriteLine($"Deleting PodId: {removedPod.PodId}");
            //    ocdb.Pods.Remove(removedPod);
            //}
            //await ocdb.SaveChangesAsync();

            var pods = ocdb.Pods
                         .Where(p => !p.Removed.HasValue).ToList()
                         .OrderByDescending(p => p.Created);
            foreach (var pod in pods)
            {
                if (_podLocks.ContainsKey(pod.PodId))
                    continue;

                if (!_podLocks.TryAdd(pod.PodId, new AsyncLock()))
                    continue;

                Debug.WriteLine($"Adding PodId: {pod.PodId} Created: {pod.Created}");
                using (var disposeIt = await _podLocks[pod.PodId].LockAsync())
                {
                    var pm = new PodModel(pod);
                    await pm.LoadAsync();
                    _podModels.Add(pm);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }

    public async Task Refresh()
    {
        try
        {
            using var ocdb = new OcdbContext();

            var pods = ocdb.Pods
                         .Where(p => !p.Removed.HasValue).ToList()
                         .OrderByDescending(p => p.Created);
            foreach (var pod in pods)
            {
                // if (pod.Created < DateTimeOffset.UtcNow - TimeSpan.FromHours(82))
                //     continue;

                if (_podLocks.ContainsKey(pod.PodId))
                    continue;

                if (!_podLocks.TryAdd(pod.PodId, new AsyncLock()))
                    continue;

                Debug.WriteLine($"Adding PodId: {pod.PodId} Created: {pod.Created}");
                using (var disposeIt = await _podLocks[pod.PodId].LockAsync())
                {
                    var pm = new PodModel(pod);
                    await pm.LoadAsync();
                    _podModels.Add(pm);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }

    public async Task Stop()
    {
        // foreach (var podLock in _podLocks.Values)
        // {
        //     await podLock.LockAsync();
        // }
    }

    public async Task<List<IPodModel>> GetPodsAsync()
    {
        return _podModels.OrderByDescending(pm => pm.Created).ToList();
    }

    public async Task RemovePodAsync(Guid podId, DateTimeOffset? removeTime = null)
    {
        if (!removeTime.HasValue)
            removeTime = DateTimeOffset.UtcNow;

        if (_podLocks.ContainsKey(podId))
        {
            _podLocks.Remove(podId, out _);
            var pm = _podModels.First(p => p.Id == podId);
            _podModels.Remove(pm);
        }
        
        using var ocdb = new OcdbContext();
        var pod = ocdb.Pods
            .First(p => p.PodId == podId);
        pod.Removed = removeTime;
        pod.IsSynced = false;
        await ocdb.SaveChangesAsync();
        _syncService.TriggerSync();
    }
    
    public async Task<Guid> NewPodAsync(
        int unitsPerMilliliter,
        MedicationType medicationType,
        uint? radioAddress)
    {
        var configuration = await _appConfiguration.Get();
        if (configuration == null)
            throw new ApplicationException("Client not registered");

        using var ocdb = new OcdbContext();
        
        var b = new byte[4];
        new Random().NextBytes(b);
        if (!radioAddress.HasValue)
        {
            radioAddress = (uint)(b[0] << 24 | b[1] << 16 | b[2] << 8 | b[3]);
        }
        var pod = new Pod
        {
            PodId = Guid.NewGuid(),
            ClientId = configuration.ClientId,
            ProfileId = configuration.DefaultProfileId,
            RadioAddress = radioAddress.Value,
            UnitsPerMilliliter = unitsPerMilliliter,
            Medication = medicationType,
        };
        ocdb.Pods.Add(pod);
        await ocdb.SaveChangesAsync();

        var pm = new PodModel(pod);
        _podLocks.TryAdd(pod.PodId, new AsyncLock());
        _podModels.Add(pm);
        _syncService.TriggerSync();
        return pod.PodId;
    }

    public async Task<Guid> ImportPodAsync(
        uint radioAddress, int unitsPerMilliliter,
        MedicationType medicationType,
        uint lot,
        uint serial
        )
    {
        
        var configuration = await _appConfiguration.Get();
        if (configuration == null)
            throw new ApplicationException("Client not registered");
        
        using var ocdb = new OcdbContext();
        var pod = await ocdb.Pods.Where(p => p.Lot == lot && p.Serial == serial).FirstOrDefaultAsync(); 
        if (pod != null)
            return pod.PodId;

        pod = new Pod
        {
            PodId = Guid.NewGuid(),
            ClientId = configuration.ClientId,
            ProfileId = configuration.DefaultProfileId, 
            RadioAddress = radioAddress,
            UnitsPerMilliliter = unitsPerMilliliter,
            Medication = medicationType,
            Lot = lot,
            Serial = serial
        };
        ocdb.Pods.Add(pod);
        await ocdb.SaveChangesAsync();
        _syncService.TriggerSync();
        var pm = new PodModel(pod);
        await pm.LoadAsync();
        _podLocks.TryAdd(pod.PodId, new AsyncLock());
        _podModels.Add(pm);
        return pod.PodId;
    }
    
    public async Task<IPodModel?> GetPodAsync(Guid podId)
    {
        return _podModels.FirstOrDefault(p => p.Id == podId);
    }

    public async Task<IPodConnection> GetConnectionAsync(
        IPodModel podModel,
        CancellationToken cancellationToken = default)
    {
        var configuration = await _appConfiguration.Get();
        if (configuration == null)
            throw new ApplicationException("Client not registered");
        
        var radioConnection = await _radioService.GetIdealConnectionAsync(cancellationToken);
        if (radioConnection == null)
            throw new ApplicationException("No radios available");

        var allocationLockDisposable = await _podLocks[podModel.Id].LockAsync(cancellationToken);
        var clientId = configuration.ClientId;

        return new PodConnection(
            clientId,
            podModel,
            radioConnection,
            allocationLockDisposable,
            _syncService);
    }
}