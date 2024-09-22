using OmniCore.Client.Abstractions.Services;
using OmniCore.Client.Services;
using OmniCore.Client.ViewModels;
using OmniCore.Client.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OmniCore.Client.ViewModels.FlyoutContentModel;

namespace OmniCore.Client;

public class AppStartup(
    FlyoutContentModel flyoutContentModel,
    NavigationModel navigationModel,
    IPlatformPermissionService platformPermissionService)
{
    public async ValueTask StartAsync()
    {
        flyoutContentModel.FlyoutItems.Add(
            new FlyoutItemModel { Title = "Home", Icon = "home" });
        flyoutContentModel.FlyoutItems.Add(
            new FlyoutItemModel { Title = "Home2", Icon = "home" });
        flyoutContentModel.FlyoutItems.Add(
            new FlyoutItemModel { Title = "Home3", Icon = "home" });

        await navigationModel.NavigateTo<RegisterClientModel>(true);
    }

    public ValueTask ResumeAsync()
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask StopAsync()
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask DestroyAsync()
    {
        return ValueTask.CompletedTask;
    }
}
