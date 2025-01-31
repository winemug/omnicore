using OmniCore.Maui.Services;
using OmniCore.Services.Interfaces.Platform;

namespace OmniCore.Maui;

public static class PlatformServicesExtension
{
    public static IServiceCollection RegisterPlatformServices(this IServiceCollection services)
    {
        services.AddSingleton<IPlatformInfo, PlatformInfo>();
        services.AddSingleton<IPlatformService, PlatformService>();
        return services;
    }
}