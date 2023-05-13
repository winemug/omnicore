using System.Diagnostics;
using System.Dynamic;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Nito.AsyncEx;
using OmniCore.Common.Data;
using OmniCore.Services.Interfaces.Amqp;
using OmniCore.Services.Interfaces.Core;
using OmniCore.Services.Interfaces.Pod;

namespace OmniCore.Services;

public class SyncService : ISyncService
{
    private IAmqpService _amqpService;
    private IConfigurationStore _configurationStore;
    private Task _syncTask;
    private CancellationTokenSource _ctsSync;
    private AsyncAutoResetEvent _syncTriggerEvent;
    public SyncService(
        IAmqpService amqpService,
        IConfigurationStore configurationStore,
        OcdbContext ocdbContext)
    {
        _amqpService = amqpService;
        _configurationStore = configurationStore;
        _ctsSync = new CancellationTokenSource();
        _syncTriggerEvent = new AsyncAutoResetEvent(true);
    }
    public async Task Start()
    {
        var cc = await _configurationStore.GetConfigurationAsync();

        await using var context = new OcdbContext();
        //context.Database.ExecuteSql($"UPDATE [Pods] SET [IsSynced] = 0");
        //context.Database.ExecuteSql($"UPDATE [PodActions] SET [IsSynced] = 0");

        var podsToSync = context.Pods.Where(p => !p.IsSynced).ToList();
        foreach (var pod in podsToSync)
        {
            await _amqpService.PublishMessage(new AmqpMessage
            {
                Text = JsonSerializer.Serialize(new
                {
                    type = "Pod",
                    data = pod
                }),
                Route = "sync",
                OnPublishConfirmed = OnPodSynced(pod.PodId),
            });
        }
        _syncTask = SyncPendingActions(_ctsSync.Token);
    }

    public async Task Stop()
    {
        _ctsSync.Cancel();
        if (_syncTask != null)
        {
            try
            {
                await _syncTask;
            }
            catch (TaskCanceledException)
            {
            }
        }
    }

    private async Task SyncPendingActions(CancellationToken cancellationToken)
    {
        while(true)
        {
            await _syncTriggerEvent.WaitAsync(cancellationToken);
            await using var context = new OcdbContext();
            var podActionsToSync = await context.PodActions.Where(pa => !pa.IsSynced).ToListAsync();
            await context.DisposeAsync();

            foreach (var podAction in podActionsToSync)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await _amqpService.PublishMessage(new AmqpMessage
                {
                    Text = JsonSerializer.Serialize(new
                    {
                        type = nameof(PodAction),
                        data = podAction
                    }),
                    Route = "sync",
                    OnPublishConfirmed = OnPodActionSynced(podAction.PodId, podAction.Index),
                });
                await Task.Yield();
            }
        }
    }

    public void TriggerSync()
    {
        _syncTriggerEvent.Set();
    }

    private async Task OnPodSynced(Guid podId)
    {
        await using var context = new OcdbContext();
        var pod = await context.Pods.FirstOrDefaultAsync(p => p.PodId == podId);
        if (pod != null)
        {
            pod.IsSynced = true;
            await context.SaveChangesAsync();
        }
    }

    private async Task OnPodActionSynced(Guid podId, int index)
    {
        await using var context = new OcdbContext();
        var podAction = await context.PodActions.FirstOrDefaultAsync(pa => pa.PodId == podId && pa.Index == index);
        if (podAction != null)
        {
            podAction.IsSynced = true;
            await context.SaveChangesAsync();
        }
    }
}