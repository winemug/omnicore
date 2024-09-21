using OmniCore.Client.ViewModels;
using OmniCore.Client.Views;
namespace OmniCore.Client;

public partial class App : Application
{
    private readonly MainModel mainModel;
    private readonly AppStartup appStartup;
    private readonly NavigationModel navigationModel;

    public App(
        MainModel mainModel,
        NavigationModel navigationModel,
        AppStartup appStartup)
    {
        InitializeComponent();

        MainPage = mainModel.Page;
        this.navigationModel = navigationModel;
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
        await navigationModel.OnWindowDeactivatedAsync();
    }
    private async void WindowOnActivated(object? sender, EventArgs e)
    {
        await navigationModel.OnWindowActivatedAsync();
    }
    private async void WindowOnStopped(object? sender, EventArgs e)
    {
        await appStartup.StopAsync();
    }
    private async void WindowOnResumed(object? sender, EventArgs e)
    {
        await appStartup.ResumeAsync();
    }
    private async void WindowOnDestroying(object? sender, EventArgs e)
    {
        await appStartup.DestroyAsync();
    }
}