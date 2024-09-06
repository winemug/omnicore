using OmniCore.Client.Abstractions.Services;
using OmniCore.Client.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmniCore.Client.Services;

public class NavigationService : IDisposable
{
    public Page MainPage { get; }
    private INavigation Navigation => ((FlyoutPage)this.MainPage).Detail.Navigation;

    private readonly IServiceProvider _serviceProvider;
    private AsyncServiceScope _serviceScope;

    public NavigationService(IServiceProvider serviceProvider)
    {
        var f = new FlyoutPage();
        var n = new NavigationPage();
        n.Title = "OmniCore";

        var c = new ContentPage();
        c.Title = "Home";

        f.Detail = n;
        f.Flyout = c;

        MainPage = f;
        _serviceProvider = serviceProvider;
        _serviceScope = _serviceProvider.CreateAsyncScope();
    }

    private void RenewScope()
    {
        _serviceScope.Dispose();
        _serviceScope = _serviceProvider.CreateAsyncScope();
    }

    public async ValueTask PushViewAsync<TView>(bool setRoot = false)
    where TView : Page
    {
        if (setRoot)
            RenewScope();
        var view = _serviceScope.ServiceProvider.GetRequiredService<TView>();
        await NavigateTo(null, view, setRoot) ;
    }

    public async ValueTask PushViewAsync<TView, TModel>(bool setRoot = false)
        where TView : Page
        where TModel : ViewModel
    {
        if (setRoot)
            RenewScope();
        var model = _serviceScope.ServiceProvider.GetRequiredService<TModel>();
        var view = _serviceScope.ServiceProvider.GetRequiredService<TView>();
        await NavigateTo(model, view, setRoot);
    }

    public async ValueTask PushDataViewAsync<TView, TModel, TModelData>(TModelData data, bool setRoot = false)
        where TView : Page
        where TModel : DataViewModel<TModelData>
        where TModelData : notnull
    {
        if (setRoot)
            RenewScope();
        var model = _serviceScope.ServiceProvider.GetRequiredService<TModel>();
        var view = _serviceScope.ServiceProvider.GetRequiredService<TView>();
        await model.LoadDataAsync(data);

        await NavigateTo(model, view, setRoot);
    }

    private async ValueTask NavigateTo(ViewModel? model, Page view, bool setRoot)
    {
        if (model != null)
        {
            await model.BindToView(view);
            await model.OnNavigatingTo();
        }
        
        if (setRoot && Navigation.NavigationStack.Count > 0)
        {
            Navigation.InsertPageBefore(view, Navigation.NavigationStack[0]);
            await Navigation.PopToRootAsync(true);
        }
        else
        {
            await Navigation.PushAsync(view, true);
        }
    }
    public ValueTask OnWindowActivatedAsync()
    {
        if (Navigation.NavigationStack.Last().BindingContext is ViewModel model)
            return model.OnResumed();
        return ValueTask.CompletedTask;
    }

    public ValueTask OnWindowDeactivatedAsync()
    {
        if (Navigation.NavigationStack.Last().BindingContext is ViewModel model)
            return model.OnPaused();
        return ValueTask.CompletedTask;
    }

    public void Dispose()
    {
        _serviceScope.Dispose();
    }
}