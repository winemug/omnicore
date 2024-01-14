using OmniCore.Client.Abstractions.Services;
using OmniCore.Client.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmniCore.Client.Services;

public class NavigationService
{
    public NavigationPage MainPage { get; }
    public INavigation Navigation => this.MainPage.Navigation;

    private ViewModel? _activeModel;
    private readonly IServiceProvider _serviceProvider;

    public NavigationService(IServiceProvider serviceProvider)
    {
        MainPage = new NavigationPage();
        _serviceProvider = serviceProvider;
        _activeModel = null;
    }

    public async ValueTask PushViewAsync<TView>(bool setRoot = false)
    where TView : Page
    {
        await NavigateAway(_activeModel);
        var view = _serviceProvider.GetRequiredService<TView>();
        await NavigateTo(null, view, setRoot) ;
    }

    public async ValueTask PushViewAsync<TView, TModel>(bool setRoot = false)
        where TView : Page
        where TModel : ViewModel
    {
        await NavigateAway(_activeModel);
        var model = _serviceProvider.GetRequiredService<TModel>();
        var view = _serviceProvider.GetRequiredService<TView>();
        await NavigateTo(model, view, setRoot);
    }

    public async ValueTask PushDataViewAsync<TView, TModel, TModelData>(TModelData data, bool setRoot = false)
        where TView : Page
        where TModel : DataViewModel<TModelData>
        where TModelData : notnull
    {
        await NavigateAway(_activeModel);

        var model = _serviceProvider.GetRequiredService<TModel>();
        var view = _serviceProvider.GetRequiredService<TView>();
        await model.LoadDataAsync(data);

        await NavigateTo(model, view, setRoot);
    }

    private async ValueTask NavigateAway(ViewModel? model)
    {
        if (model != null)
        {
            await model.OnNavigatingAway();
            if (model is IAsyncDisposable asyncDisposable)
                await asyncDisposable.DisposeAsync();
            if (model is IDisposable disposable)
                disposable.Dispose();
        }
    }

    private async ValueTask NavigateTo(ViewModel? model, Page view, bool setRoot)
    {
        if (model != null)
        {
            await model.BindToView(view);
            await model.OnNavigatingTo();
        }
        _activeModel = model;
        
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
        if (_activeModel != null)
            return _activeModel.OnResumed();
        return ValueTask.CompletedTask;
    }

    public ValueTask OnWindowDeactivatedAsync()
    {
        if (_activeModel != null)
            return _activeModel.OnPaused();
        return ValueTask.CompletedTask;
    }
}