using Microsoft.Extensions.Logging;
using OmniCore.Client.Platforms;
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
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });
#if DEBUG
		builder.Logging.AddDebug();
#endif

        builder.Services
            .AddSingleton<AppStartup>()
            .AddSingleton<MainModel>().AddSingleton<FlyoutPage>()
            .AddSingleton<FlyoutContentModel>().AddSingleton<FlyoutContentPage>()
            .AddSingleton<NavigationModel>().AddSingleton<NavigationPage>()

            .AddScoped<RegisterClientModel>().AddScoped<RegisterClientPage>()
            .AddScoped<CreateAccountModel>().AddScoped<CreateAccountPage>()
            .AddScoped<HomeModel>().AddScoped<HomePage>()

            .RegisterPlatformServices();


        return builder.Build();
    }
}
