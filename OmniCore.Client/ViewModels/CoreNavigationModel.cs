using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using OmniCore.Client.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OmniCore.Client.ViewModels;

public class CoreNavigationModel : ViewModel, IDisposable
{

    public ICommand FlyoutSelectionChangedCommand => new RelayCommand(FlyoutSelectionChanged);
    public ObservableCollection<FlyoutItemModel> FlyoutItems { get; set; }

    private readonly IServiceProvider serviceProvider;
    private IServiceScope scope;
    private NavigationPage navigationPage;
    private FlyoutPage flyoutPage;

    public CoreNavigationModel(CoreNavigationPage page,
        IServiceProvider serviceProvider)
        : base(page)
    {
        var flyoutPage = new FlyoutPage();
        flyoutPage.Title = "OmniCore Flyout";

        var navigationPage = new NavigationPage();
        navigationPage.Title = "OmniCore";
        flyoutPage.Detail = navigationPage;

        flyoutPage.FlyoutLayoutBehavior = FlyoutLayoutBehavior.Popover;
        flyoutPage.IsPresented = true;

        this.flyoutPage = flyoutPage;
        this.navigationPage = navigationPage;

        this.serviceProvider = serviceProvider;
        this.scope = serviceProvider.CreateScope();

        FlyoutItems = new ObservableCollection<FlyoutItemModel>();
    }
    private void FlyoutSelectionChanged()
    {
        Console.WriteLine($"{DateTimeOffset.UtcNow} FlyoutSelectionChanged");
    }

    public ValueTask OnWindowDeactivatedAsync()
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask OnWindowActivatedAsync()
    {
        return ValueTask.CompletedTask;
    }

    //if (model != null)
    //{
    //    await model.BindToView(view);
    //    await model.OnNavigatingTo();
    //}

    //if (setRoot && Navigation.NavigationStack.Count > 0)
    //{
    //    Navigation.InsertPageBefore(view, Navigation.NavigationStack[0]);
    //    await Navigation.PopToRootAsync(true);
    //}
    //else
    //{
    //    await Navigation.PushAsync(view, true);
    //}

    //public async ValueTask PushAsync<TModel>(bool setRoot = false)
    //where TModel : ViewModel
    //{
    //    if (setRoot)
    //        RenewScope();
    //    var model = this.serviceScope.ServiceProvider.GetRequiredService<TModel>();
    //    var view = _serviceScope.ServiceProvider.GetRequiredService<TView>();
    //    await NavigateTo(model, view, setRoot);
    //}

    //public async ValueTask PushDataViewAsync<TView, TModel, TModelData>(TModelData data, bool setRoot = false)
    //    where TView : Page
    //    where TModel : DataViewModel<TModelData>
    //    where TModelData : notnull
    //{
    //    if (setRoot)
    //        RenewScope();
    //    var model = _serviceScope.ServiceProvider.GetRequiredService<TModel>();
    //    var view = _serviceScope.ServiceProvider.GetRequiredService<TView>();
    //    await model.LoadDataAsync(data);

    //    await NavigateTo(model, view, setRoot);
    //}

    public void Dispose()
    {
        scope.Dispose();
    }
}

public class FlyoutItemModel
{
    public required string Title { get; set; }
    public required string Icon { get; set; }
}
