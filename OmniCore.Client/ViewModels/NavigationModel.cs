using Microsoft.Maui.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmniCore.Client.ViewModels;

public class NavigationModel : ViewModel, IDisposable
{
    private readonly NavigationPage navigationPage;
    private readonly FlyoutContentModel flyoutContentModel;
    private readonly IServiceProvider serviceProvider;
    private IServiceScope serviceScope;

    public Page FlyoutPage { get; }

    public NavigationModel(
        NavigationPage page,
        FlyoutContentModel flyoutContentModel,
        IServiceProvider serviceProvider)
        : base(page)
    {
        page.Title = "Main Navigation";
        this.flyoutContentModel = flyoutContentModel;
        this.serviceProvider = serviceProvider;
        this.serviceScope = serviceProvider.CreateAsyncScope();

        FlyoutPage = flyoutContentModel.Page;
    }

    public async ValueTask Start<TViewModel>() where TViewModel : ViewModel
    {
    }

    public async ValueTask NavigateTo<TViewModel>(bool popToRoot) where TViewModel : ViewModel
    {
        if (popToRoot)
        {
            this.serviceScope.Dispose();
            this.serviceScope = serviceProvider.CreateAsyncScope();
        }

        var viewModel = serviceScope.ServiceProvider.GetRequiredService<TViewModel>();

        if (popToRoot && Page.Navigation.NavigationStack.Count > 0)
        { 
            Page.Navigation.InsertPageBefore(viewModel.Page, Page.Navigation.NavigationStack[0]);
            await Page.Navigation.PopToRootAsync(true);
        }
        else
        {
            await Page.Navigation.PushAsync(viewModel.Page);
        }
    }

    public ValueTask OnWindowDeactivatedAsync()
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask OnWindowActivatedAsync()
    {
        return ValueTask.CompletedTask;
    }

    public void Dispose()
    {
        serviceScope.Dispose();
    }
}
