using CommunityToolkit.Mvvm.ComponentModel;
using OmniCore.Services.Interfaces.Platform;

namespace OmniCore.Maui.Models;

public class WelcomeModel(IPlatformService platformService, IPlatformInfo platformInfo)
    : ObservableObject
{
}