using OmniCore.Client.Views;

namespace OmniCore.Client.ViewModels;

public class MainModel : ViewModel
{
    public MainModel(FlyoutPage flyoutPage, NavigationModel navigationModel)
        : base(flyoutPage)
    {
        flyoutPage.Title = "Flyout flyout out";
        flyoutPage.FlyoutLayoutBehavior = FlyoutLayoutBehavior.Popover;
        flyoutPage.IsPresented = true;
        flyoutPage.IsGestureEnabled = true;

        flyoutPage.Flyout = navigationModel.FlyoutPage;
        flyoutPage.Detail = navigationModel.Page;
    }

}