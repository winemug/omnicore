using CommunityToolkit.Mvvm.Input;
using OmniCore.Client.Abstractions.Services;
using OmniCore.Client.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OmniCore.Client.ViewModels;

public class PermissionsModel(NavigationService navigationService, IPlatformPermissionService platformPermissionService) : ViewModel
{
    public ICommand BluetoothCommand => new RelayCommand(RequestBluetoothPermissionsCommand);
    public ICommand NotificationsCommand => new RelayCommand(RequestNotificationPermissionCommand);
    public ICommand BackgroundDataCommand => new RelayCommand(RequestBackgroundDataPermissionCommand);
    public ICommand BatteryExemptionCommand => new RelayCommand(RequestBatteryExemptionCommand);

    public bool BluetoothPermissionsRequired { get; set; }
    public bool NotificationPermissionsRequired { get; set; }
    public bool BackgroundDataPermissionsRequired { get; set; }
    public bool BatteryExemptionRequired { get; set; }  


    public override async ValueTask OnNavigatingTo()
    {
        BluetoothPermissionsRequired = await platformPermissionService.RequiresBluetoothPermissionAsync();
        NotificationPermissionsRequired = await platformPermissionService.RequiresForegroundPermissionAsync();
        BackgroundDataPermissionsRequired = await platformPermissionService.IsBackgroundDataRestrictedAsync(); 
        BatteryExemptionRequired = await platformPermissionService.IsBatteryOptimizedAsync();
    }

    public async void RequestBluetoothPermissionsCommand()
    {
        BluetoothPermissionsRequired = await platformPermissionService.RequestBluetoothPermissionAsync();
    }

    public async void RequestNotificationPermissionCommand()
    {
        NotificationPermissionsRequired = await platformPermissionService.RequestForegroundPermissionAsync();
    }

    public async void RequestBackgroundDataPermissionCommand()
    {
        BackgroundDataPermissionsRequired = await platformPermissionService.TryExemptFromBackgroundDataRestriction();
    }

    public async void RequestBatteryExemptionCommand()
    {
        BatteryExemptionRequired = await platformPermissionService.TryExemptFromBatteryOptimization();
    }

}
