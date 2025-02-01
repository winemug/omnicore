using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace OmniCore.Maui.Models;

public partial class PermissionsModel : PageModel
{
    [ObservableProperty] List<DynamicPermission> _dynamicPermissions =
    [
        new DynamicPermission("android.permission.ACCESS_NETWORK_STATE", false),
        new DynamicPermission("android.permission.INTERNET", false),
        new DynamicPermission("android.permission.BLUETOOTH_CONNECT", false),
        new DynamicPermission("android.permission.BLUETOOTH_SCAN", false),
        new DynamicPermission("android.permission.POST_NOTIFICATIONS", false),
        new DynamicPermission("android.permission.FOREGROUND_SERVICE", false),
        new DynamicPermission("android.permission.FOREGROUND_SERVICE_SYSTEM_EXEMPTED", false),
        new DynamicPermission("android.permission.SCHEDULE_EXACT_ALARM", false),
    ];

    protected override async Task Appearing()
    {
        await Check();
        base.OnPropertyChanged(nameof(DynamicPermissions));
    }

    private async Task Check()
    {
        foreach (var permission in _dynamicPermissions)
        {
            await permission.UpdateStatusAsync();
        }
    }
}
public class DynamicPermission(string permissionName, bool isRuntime) : Permissions.BasePlatformPermission
{
    private PermissionStatus _lastStatus = PermissionStatus.Unknown;
    public string ShortName => permissionName.Substring(permissionName.LastIndexOf('.') + 1);
    public bool IsGranted => _lastStatus == PermissionStatus.Granted;
    public override string ToString()
    {
        return $"{permissionName}, {isRuntime} = {_lastStatus}";
    }
    public override (string androidPermission, bool isRuntime)[] RequiredPermissions => new[]
    {
        (permissionName, isRuntime),
    };

    public async Task UpdateStatusAsync()
    {
        _lastStatus = await CheckStatusAsync();
    }

    public async Task RequestAndUpdateStatusAsync()
    {
        _lastStatus = await RequestAsync();
    }

}
