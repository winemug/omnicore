using CommunityToolkit.Maui;
using Fonts;
using Microsoft.Extensions.Logging;
using OmniCore.Services.Interfaces.Platform;

namespace OmniCore.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp(IPlatformInfo platformInfo, IPlatformService platformService)
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .RegisterAppServices(platformInfo, platformService)
            .ConfigureMauiHandlers(handlers =>
            {
            })
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("SegoeUI-Semibold.ttf", "SegoeSemibold");
                fonts.AddFont("FluentSystemIcons-Regular.ttf", FluentUI.FontFamily);
            });

#if DEBUG
        builder.Logging.AddDebug();
        builder.Services.AddLogging(configure => configure.AddDebug());
#endif

        return builder.Build();
    }

    public static MauiAppBuilder RegisterAppServices(this MauiAppBuilder mauiAppBuilder,
        IPlatformInfo platformInfo, IPlatformService platformService)
    {
        mauiAppBuilder.Services.AddSingleton<IPlatformService>(platformService);
        mauiAppBuilder.Services.AddSingleton<IPlatformInfo>(platformInfo);

        return mauiAppBuilder;
    }
}
