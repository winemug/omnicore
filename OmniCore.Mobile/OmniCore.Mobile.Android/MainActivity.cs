﻿using System;

using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using OmniCore.Services.Interfaces;
using Unity;
using Unity.Injection;
using Xamarin.Forms;


namespace OmniCore.Mobile.Droid
{
    [Activity(Label = "OmniCore", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private PlatformInfo _platformInfo;
        private ForegroundServiceHelper _foregroundServiceHelper;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            AndroidEnvironment.UnhandledExceptionRaiser += (sender, args) => {
                var newExc = new ApplicationException("AndroidEnvironment_UnhandledExceptionRaiser", args.Exception);
            };
            //App.Container.RegisterType<IPlatformInfo, PlatformInfo>(new InjectionConstructor(this));
            _platformInfo = new PlatformInfo(this, this);
            _foregroundServiceHelper = new ForegroundServiceHelper(this);
            DependencyService.RegisterSingleton<IPlatformInfo>(_platformInfo);
            DependencyService.RegisterSingleton<IForegroundServiceHelper>(_foregroundServiceHelper);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            _platformInfo.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}