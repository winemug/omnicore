using OmniCore.Client.Abstractions.Services;
using OmniCore.Client.Services;
using OmniCore.Client.ViewModels;
using OmniCore.Client.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmniCore.Client;

public class AppStartup(NavigationService navigationService, IPlatformPermissionService platformPermissionService)
{
    public async Task StartAsync()
    {
        if (await platformPermissionService.RequiresBluetoothPermissionAsync() ||
            await platformPermissionService.RequiresForegroundPermissionAsync() || 
            await platformPermissionService.IsBackgroundDataRestrictedAsync() ||
            await platformPermissionService.IsBatteryOptimizedAsync())
        {
            await navigationService.PushViewAsync<PermissionsPage, PermissionsModel>(setRoot: false);
        }
    }
}
