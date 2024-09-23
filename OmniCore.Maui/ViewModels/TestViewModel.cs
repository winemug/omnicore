using System.Diagnostics;
using System.Windows.Input;
using OmniCore.Common.Api;
using OmniCore.Common.Data;
using OmniCore.Framework.Omnipod.Requests;
using OmniCore.Services;
using OmniCore.Services.Interfaces;
using OmniCore.Services.Interfaces.Amqp;
using OmniCore.Services.Interfaces.Core;
using OmniCore.Services.Interfaces.Entities;
using OmniCore.Services.Interfaces.Platform;
using OmniCore.Services.Interfaces.Pod;
using OmniCore.Shared.Api;
using Org.Apache.Http.Client.Params;

namespace OmniCore.Maui.ViewModels;
public class TestViewModel : BaseViewModel
{
    public string StatusText { get; set; }

    public ICommand StopCommand { get; set; }

    private IPlatformService _platformService;
    private IPlatformInfo _platformInfo;
    private IAmqpService _amqpService;
    private IApiClient _apiClient;
    private IPodService _podService;
    private IRadioService _radioService;
    private IAppConfiguration _appConfiguration;

    public TestViewModel(
        IAppConfiguration appConfiguration,
        IPlatformService platformService,
        IPlatformInfo platformInfo,
        IAmqpService amqpService,
        IApiClient apiClient,
        IPodService podService,
        IRadioService radioService)
    {
        _appConfiguration = appConfiguration;
        _platformService = platformService;
        _platformInfo = platformInfo;
        _amqpService = amqpService;
        _apiClient = apiClient;
        _podService = podService;
        _radioService = radioService;

        StopCommand = new Command(async () => await ExecuteStop());
        // NewPodCommand = new Command(async () => await ExecuteNewPod());
        // PrimeCommand = new Command(async () => await ExecutePrime());
        // StartCommand = new Command(async () => await ExecuteStartPod());
    }

    private bool appearOnce = false;
    public override async ValueTask OnAppearing()
    {
        if (appearOnce)
            return;
        appearOnce = true;
        await _platformInfo.VerifyPermissions();

        using var context = new OcdbContext();
        await context.Database.EnsureCreatedAsync();
        await _appConfiguration.Set(new OmniCoreConfiguration
        {
            AmqpConnectionString = "amqps://amqp.balya.net/ocv",
            AccountId = new Guid("269d7830-fe9b-4641-8123-931846e45c9c"),
            ClientId = new Guid("ee843c96-a312-4d4b-b0cc-93e22d6e680e"),
            DefaultProfileId = new Guid("7d799596-3f6d-48e2-ac65-33ca6396788b"),
            UserId = "occ",
            RequestExchange = "e_requests",
            ResponseExchange = "e_responses",
            SyncExchange = "e_sync",
            ClientCertificate = "",
            ClientKey = ""

        });
        _platformService.StartService();
        // await _radioService.Start();
        // await _podService.Start();
        // await _amqpService.Start();
    }
 
    private async Task ExecuteStop()
    {
        _platformService.StopService();
    }

    // private async Task ExecuteNewPod()
    // {
    //     var podId = await _podService.NewPodAsync(new Guid("7D799596-3F6D-48E2-AC65-33CA6396788B"), 100, MedicationType.Insulin, null);
    //     // var pods = await _podService.GetPodsAsync();
    //     // var pod = pods[1];
    //     // using (var pc = await _podService.GetConnectionAsync(pod))
    //     // {
    //     //     await pc.Deactivate();
    //     // }
    // }
    //
    // private async Task ExecutePrime()
    // {
    //     var pods = await _podService.GetPodsAsync();
    //     var pod = pods[0];
    //     using (var pc = await _podService.GetConnectionAsync(pod))
    //     {
    //         var now = DateTime.Now;
    //         var res = await pc.PrimePodAsync(new DateOnly(now.Year, now.Month, now.Day),
    //             new TimeOnly(now.Hour, now.Minute, now.Second),
    //             true, CancellationToken.None);
    //     }
    // }
    //
    // private async Task ExecuteStartPod()
    // {
    //     var pods = await _podService.GetPodsAsync();
    //     var pod = pods[0];
    //     using (var pc = await _podService.GetConnectionAsync(pod))
    //     {
    //         var now = DateTime.Now;
    //         var basalRateTicks = new int[48];
    //         for (int i = 0; i < 48; i++)
    //             basalRateTicks[i] = 12;
    //
    //         var res = await pc.StartPodAsync(
    //             new TimeOnly(now.Hour, now.Minute, now.Second), basalRateTicks);
    //     }
    // }

}