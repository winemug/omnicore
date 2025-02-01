using System.Diagnostics;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace OmniCore.Maui.Models;

public partial class PermissionsModel : PageModel
{
    [ObservableProperty] List<DynamicPermission> _dynamicPermissions =
    [
        new DynamicPermission(permissionName: "android.permission.INTERNET", true),
        new DynamicPermission(permissionName: "android.permission.ACCESS_NETWORK_STATE", true),
        new DynamicPermission(permissionName: "android.permission.CHANGE_NETWORK_STATE", true),
        
        new DynamicPermission(permissionName: "android.permission.BLUETOOTH_CONNECT", true),
        new DynamicPermission(permissionName: "android.permission.BLUETOOTH_SCAN", true),
        
        new DynamicPermission(permissionName: "android.permission.POST_NOTIFICATIONS", true),
        new DynamicPermission(permissionName: "android.permission.FOREGROUND_SERVICE", true),
        new DynamicPermission(permissionName: "android.permission.FOREGROUND_SERVICE_CONNECTED_DEVICE", true),
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
public class DynamicPermission : Permissions.BasePlatformPermission
{
    private readonly string _permissionName;
    private readonly bool _isRuntime;

    public DynamicPermission(string permissionName, bool isRuntime)
    {
        _permissionName = permissionName;
        _isRuntime = isRuntime;
        RequestAndUpdateCommand = new RelayCommand(async () =>
        {
            await RequestAndUpdateStatusAsync();
        });
    }
    private PermissionStatus _lastStatus = PermissionStatus.Unknown;
    public string ShortName => _permissionName.Substring(_permissionName.LastIndexOf('.') + 1);
    public bool IsGranted => _lastStatus == PermissionStatus.Granted;
    public ICommand RequestAndUpdateCommand { get; }
    public override (string androidPermission, bool isRuntime)[] RequiredPermissions => new[]
    {
        (_permissionName, _isRuntime),
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
