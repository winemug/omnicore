﻿using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using OmniCore.Client.Interfaces.Services;
using OmniCore.Client.Mobile.Services;
using OmniCore.Client.Mobile.ViewModels;
using OmniCore.Client.Mobile.Views;

namespace OmniCore.Client.Mobile
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
		builder.Logging.AddDebug();
#endif

            builder.Services
                .AddViewViewModel<PermissionsPage, PermissionsViewModel>()
                .AddViewViewModel<AccountLoginPage, AccountLoginViewModel>()

                .AddSingleton<ICoreService, CoreService>()
                .AddSingleton<INavigationService, NavigationService>();

            builder.Services.RegisterPlatformServices();

            return builder.Build();
        }
    }
}