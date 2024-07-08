using System.Text.Json;
using OmniCore.Services.Interfaces.Core;
using OmniCore.Services.Interfaces.Platform;
using OmniCore.Shared.Extensions;

namespace OmniCore.Maui.Services;

public class AppConfiguration : IAppConfiguration
{
    private IPlatformInfo _platformInfo;
    private OmniCoreConfiguration? _configuration;
    public AppConfiguration(IPlatformInfo platformInfo)
    {
        _platformInfo = platformInfo;
        var val = Preferences.Get(nameof(OmniCoreConfiguration), null);
        _configuration = JsonSerializerWrapper.TryDeserialize<OmniCoreConfiguration>(val);
    }

    public async Task Set(OmniCoreConfiguration configuration)
    {
        var strVal = JsonSerializerWrapper.TrySerialize(configuration);
        Preferences.Set(nameof(OmniCoreConfiguration), strVal);
        _configuration = configuration;
    }

    public async Task<OmniCoreConfiguration?> Get()
    {
        return _configuration;
    }
}