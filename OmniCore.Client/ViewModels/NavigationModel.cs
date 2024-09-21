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
        page.NavigatedTo += async (s, e) => await OnWindowActivatedAsync();
        page.NavigatedFrom += async (s, e) => await OnWindowDeactivatedAsync();
        this.flyoutContentModel = flyoutContentModel;
        this.serviceProvider = serviceProvider;
        this.serviceScope = serviceProvider.CreateAsyncScope();

        FlyoutPage = flyoutContentModel.Page;
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

    private ViewModel? GetActiveViewModel()
    {
        if (Page.Navigation.NavigationStack.Count == 0)
            return null;
        return Page.Navigation.NavigationStack.Last().BindingContext as ViewModel;
    }
    public ValueTask OnWindowDeactivatedAsync()
    {
        var model = GetActiveViewModel();
        if (model == null)
            return ValueTask.CompletedTask;

        return model.OnDisappear();
    }

    public ValueTask OnWindowActivatedAsync()
    {
        var model = GetActiveViewModel();
        if (model == null)
            return ValueTask.CompletedTask;

        return model.OnAppear();
    }

    public override void Dispose()
    {
        serviceScope.Dispose();
        base.Dispose();
    }
}
