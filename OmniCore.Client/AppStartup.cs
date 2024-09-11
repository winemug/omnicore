using OmniCore.Client.Abstractions.Services;
using OmniCore.Client.ViewModels;
using OmniCore.Client.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmniCore.Client;

public class AppStartup(CoreNavigationModel navigationModel,
    IPlatformPermissionService platformPermissionService)
{
    public ValueTask StartAsync()
    {
        navigationModel.FlyoutItems.Add(new FlyoutItemModel
        {
            Title = "Home",
            Icon = "home",
        });
        return ValueTask.CompletedTask;
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
