using Android.Content;
using Android.OS;
using OmniCore.Services.Interfaces.Platform;
using static Microsoft.Maui.ApplicationModel.Permissions;
using Debug = System.Diagnostics.Debug;

namespace OmniCore.Maui.Services
{
    public class PlatformInfo : IPlatformInfo
    {
        //public partial string SoftwareVersion { get; }
        //public partial string HardwareVersion { get; }
        //public partial string OsVersion { get; }
        //public partial string Platform { get; }

        public async Task<bool> VerifyPermissions()
        {
            await CheckAndRequest<Permissions.LocationAlways>();
            await CheckAndRequest<BluetoothPermissions>();
            await CheckAndRequest<ForegroundPermissions>();
            await CheckAndRequest<NotificationPermissions>();
            await CheckAndRequest<Permissions.StorageWrite>();
            await CheckAndRequest<Permissions.StorageRead>();
            
            var pm = (PowerManager)MauiApplication.Current.GetSystemService(Context.PowerService);
            if (pm.IsIgnoringBatteryOptimizations(MauiApplication.Current.PackageName))
            {
                return true;
            }

            AppInfo.Current.ShowSettingsUI();

            // var intent = new Intent();
            // intent.SetAction(Settings.ActionIgnoreBatteryOptimizationSettings);
            // MauiApplication.Current.StartActivity(intent);
            return false;
        }

        //public PlatformInfo()
        //{
        //    Platform = "Android";
        //    HardwareVersion = "1.0.0";
        //    SoftwareVersion = "1.0.0";
        //    OsVersion = "10";
        //}

        private async Task<PermissionStatus> CheckAndRequest<T>() where T : BasePermission, new()
        { 
            Debug.WriteLine($"Checking permission {typeof(T)}");
            var status = await Permissions.CheckStatusAsync<T>();
            Debug.WriteLine($"Permission status: {status}");
            if (status != PermissionStatus.Granted)
            {
                Debug.WriteLine($"Requesting permission");
                status = await Permissions.RequestAsync<T>();
                Debug.WriteLine($"Request result: {status}");
            }

            return status;
        }
    }
    public class BluetoothPermissions : Permissions.BasePlatformPermission
    {
        public override (string androidPermission, bool isRuntime)[] RequiredPermissions =>
            new List<(string androidPermission, bool isRuntime)>
            {
                (global::Android.Manifest.Permission.Bluetooth, false),
                (global::Android.Manifest.Permission.BluetoothAdmin, false),
                (global::Android.Manifest.Permission.BluetoothConnect, false),
                (global::Android.Manifest.Permission.BluetoothScan, false)
            }.ToArray();
    }
    
    public class ForegroundPermissions : Permissions.BasePlatformPermission
    {
        public override (string androidPermission, bool isRuntime)[] RequiredPermissions =>
            new List<(string androidPermission, bool isRuntime)>
            {
                (global::Android.Manifest.Permission.ForegroundService, true),
            }.ToArray();
    }
    
    public class NotificationPermissions : Permissions.BasePlatformPermission
    {
        public override (string androidPermission, bool isRuntime)[] RequiredPermissions =>
            new List<(string androidPermission, bool isRuntime)>
            {
                (global::Android.Manifest.Permission.PostNotifications, true),
            }.ToArray();
    }
}