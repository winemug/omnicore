using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace OmniCore.Maui.Models;

public partial class PermissionsModel : PageModel
{
    [ObservableProperty] PermissionStatus? _permission1Status;
    [ObservableProperty] PermissionStatus? _permission2Status;

    protected override async Task Appearing()
    {
        await Check();
    }

    private async Task Check()
    {
        Permission1Status = await Permissions.CheckStatusAsync<Permissions.Bluetooth>();
        Permission2Status = await Permissions.CheckStatusAsync<Permissions.PostNotifications>();
    }
    [RelayCommand]
    private async Task RequestPermission1()
    {
        Permission1Status = await Permissions.RequestAsync<Permissions.Bluetooth>();
    }
    
    [RelayCommand]
    private async Task RequestPermission2()
    {
        Permission2Status = await Permissions.RequestAsync<Permissions.PostNotifications>();
    }

}