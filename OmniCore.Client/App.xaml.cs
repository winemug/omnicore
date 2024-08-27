using OmniCore.Client.Services;
using OmniCore.Client.ViewModels;
using OmniCore.Client.Views;

namespace OmniCore.Client;

public partial class App : Application
{
    private readonly NavigationService navigationService;
    private readonly AppStartup appStartup;

    public App(NavigationService navigationService, AppStartup appStartup)
    {
        InitializeComponent();

        MainPage = navigationService.MainPage;
        this.navigationService = navigationService;
        this.appStartup = appStartup;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = base.CreateWindow(activationState);

        window.Created += WindowOnCreated;
        window.Activated += WindowOnActivated;
        window.Deactivated += WindowOnDeactivated;
        window.Stopped += WindowOnStopped;
        window.Resumed += WindowOnResumed;
        window.Destroying += WindowOnDestroying;
        return window;
    }

    private async void WindowOnCreated(object? sender, EventArgs e)
    {
        await appStartup.StartAsync();
    }
    private async void WindowOnDeactivated(object? sender, EventArgs e)
    {
        await navigationService.OnWindowDeactivatedAsync();
    }
    private async void WindowOnActivated(object? sender, EventArgs e)
    {
        await navigationService.OnWindowActivatedAsync();
    }
    private void WindowOnStopped(object? sender, EventArgs e)
    {
    }
    private void WindowOnResumed(object? sender, EventArgs e)
    {
    }
    private void WindowOnDestroying(object? sender, EventArgs e)
    {
    }
}