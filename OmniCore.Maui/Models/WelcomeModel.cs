using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OmniCore.Services.Interfaces.Platform;

namespace OmniCore.Maui.Models;

public partial class WelcomeModel(IPlatformService platformService, IPlatformInfo platformInfo)
    : ObservableObject
{

    private bool _isNavigatedTo;

    [RelayCommand]
    private void NavigatedTo() =>
        _isNavigatedTo = true;

    [RelayCommand]
    private void NavigatedFrom() =>
        _isNavigatedTo = false;

    [RelayCommand]
    private async Task Appearing()
    {
        if (_isNavigatedTo)
        {
            // refresh etc
        }
    }
    
    [RelayCommand]
    private async Task Kommando()
    {
    }

}