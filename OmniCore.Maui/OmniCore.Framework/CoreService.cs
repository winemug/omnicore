using System.Diagnostics;
using System.Threading.Tasks;
using OmniCore.Services.Interfaces;
using OmniCore.Services.Interfaces.Core;

namespace OmniCore.Services;

public class CoreService : ICoreService
{
    public IRadioService RadioService { get; set; }

    public IAmqpService AmqpService { get; set; }

    public IPodService PodService { get; set; }

    public IDataService DataService { get; set; }

    public CoreService(IRadioService radioService,
        IAmqpService amqpService,
        IPodService podService,
        IDataService dataService)
    {
        RadioService = radioService;
        AmqpService = amqpService;
        PodService = podService;
        DataService = dataService;
    }

    public async Task Start()
    {
        Debug.WriteLine("Core services starting");

        await Task.WhenAll(
            DataService.Start()
            );
        await Task.WhenAll(
            RadioService.Start(),
            PodService.Start(),
            AmqpService.Start()
        );
        Debug.WriteLine("Core services started");
    }

    public async Task Stop()
    {
        Debug.WriteLine("Core services stopping");
        await Task.WhenAll(
            RadioService.Stop(),
            PodService.Stop(),
            AmqpService.Stop()
        );
        await Task.WhenAll(
            DataService.Stop()
        );
        Debug.WriteLine("Core services stopped");
    }
}
