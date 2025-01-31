using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OmniCore.Services.Interfaces.Platform;

namespace OmniCore.Maui.Models;

public partial class WelcomeModel(IPlatformService platformService, IPlatformInfo platformInfo)
    : PageModel
{
    [RelayCommand]
    private async Task Kommando()
    {
    }

}