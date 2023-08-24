using System.Diagnostics;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.Threading;
using OmniCore.Common.Amqp;
using OmniCore.Common.Core;
using OmniCore.Common.Pod;
using OmniCore.Framework.Omnipod;

namespace OmniCore.Framework;

public class RaddService : BackgroundService, IRaddService
{
    private readonly IAmqpService _amqpService;
    private readonly IPodService _podService;
    private readonly IRadioService _radioService;

    public RaddService(
        IPodService podService,
        IAmqpService amqpService,
        IRadioService radioService)
    {
        _podService = podService;
        _amqpService = amqpService;
        _radioService = radioService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await stoppingToken.WaitHandle;
    }

    public async Task<bool> ProcessMessageAsync(AmqpMessage message)
    {
        var rr = JsonSerializer.Deserialize<RaddRequest>(message.Text);
        if (rr == null)
            return false;

        if (string.IsNullOrEmpty(rr.pod_id))
        {
            if (rr.transfer_active_serial.HasValue && rr.transfer_active_lot.HasValue)
            {
                uint? acquired_address = null;
                if (!rr.transfer_active_address.HasValue)
                {
                    using var rc = await _radioService.GetIdealConnectionAsync();
                    for (var k = 0; k < 3; k++)
                    {
                        for (var i = 0; i < 10; i++)
                        {
                            var bler = await rc.TryGetPacket(0, 1000);
                            var packet = PodPacket.FromExchangeResult(bler);
                            if (packet != null)
                            {
                                Debug.WriteLine($"Packet: {packet}");
                                if (acquired_address.HasValue && acquired_address.Value != packet.Address) break;
                                acquired_address = packet.Address;
                            }
                        }

                        if (acquired_address.HasValue)
                            break;
                    }

                    if (!acquired_address.HasValue)
                    {
                        var msg = new AmqpMessage
                        {
                            Text = JsonSerializer.Serialize(
                                new
                                {
                                    rr.request_id,
                                    success = false
                                })
                        };
                        _amqpService.PublishMessage(msg);
                        return true;
                    }

                    rr.transfer_active_address = acquired_address;

                    while (true)
                    {
                        var packet = await rc.TryGetPacket(0, 5000);
                        if (packet == null)
                            break;
                    }
                }
            }

            var pods = await _podService.GetPodsAsync();
            var podsmsg = new AmqpMessage
            {
                Text = JsonSerializer.Serialize(
                    new
                    {
                        rr.request_id,
                        pod_ids = pods.Select(p => p.Id.ToString("N")).ToList(),
                        success = true
                    })
            };
            _amqpService.PublishMessage(podsmsg);
            return true;
        }


        var pod = await _podService.GetPodAsync(Guid.Parse(rr.pod_id));
        var success = pod != null;
        if (success)
            using (var podConnection = await _podService.GetConnectionAsync(pod))
            {
                if (success && rr.next_record_index != null && rr.next_record_index != 0)
                    success = pod.NextRecordIndex == rr.next_record_index.Value;

                if (success && rr.update_status)
                {
                    var response = await podConnection.UpdateStatus();
                    success = response == PodRequestStatus.Executed;
                }

                if (success && rr.beep)
                {
                    var response = await podConnection.Beep(BeepType.BipBip);
                    success = response == PodRequestStatus.Executed;
                }

                if (success && rr.cancel_bolus)
                {
                    var response = await podConnection.CancelBolus();
                    success = response == PodRequestStatus.Executed;
                }

                if (success && rr.cancel_temp)
                {
                    var response = await podConnection.CancelTempBasal();
                    success = response == PodRequestStatus.Executed;
                }

                if (success && rr.temp_basal_ticks.HasValue && rr.temp_basal_half_hours.HasValue)
                {
                    var response =
                        await podConnection.SetTempBasal(rr.temp_basal_ticks.Value, rr.temp_basal_half_hours.Value);
                    success = response == PodRequestStatus.Executed;
                }

                if (success && rr.bolus_ticks is > 0 && !rr.test_bolus)
                {
                    var response = await podConnection.Bolus((int)rr.bolus_ticks, 2000);
                    success = response == PodRequestStatus.Executed;
                }

                if (success && rr.deactivate)
                {
                    var response = await podConnection.Deactivate();
                    success = response == PodRequestStatus.Executed;
                }

                if (success && rr.remove)
                {
                    success = false;
                    try
                    {
                        await _podService.RemovePodAsync(Guid.Parse(rr.pod_id));
                        success = true;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }

                if (success && rr.test_bolus && rr.bolus_ticks is > 0)
                    await podConnection.Bolus((int)rr.bolus_ticks, 2000, true);
            }

        var resp = new RaddResponse
        {
            success = success,
            request_id = rr.request_id,
            next_record_index = pod?.NextRecordIndex,
            minutes = pod?.StatusModel?.ActiveMinutes,
            remaining = pod?.StatusModel?.PulsesRemaining,
            delivered = pod?.StatusModel?.PulsesDelivered
        };
        var respMessage = new AmqpMessage { Text = JsonSerializer.Serialize(resp) };
        _amqpService.PublishMessage(respMessage);

        return true;
    }
}