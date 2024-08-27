using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using OmniCore.Client.Platforms;
using OmniCore.Client.Services;
using OmniCore.Client.ViewModels;
using OmniCore.Client.Views;

namespace OmniCore.Client;

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
            .AddSingleton<NavigationService>()
            .AddSingleton<AppStartup>()

            .AddTransient<EmptyPage>()
            .AddTransient<AuthenticationModel>().AddTransient<AuthenticationPage>()
            .AddTransient<PermissionsModel>().AddTransient<PermissionsPage>()

            .RegisterPlatformServices();


        return builder.Build();
    }
}
