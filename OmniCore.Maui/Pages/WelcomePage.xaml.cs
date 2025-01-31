using Android.Media;
using OmniCore.Maui.Models;

namespace OmniCore.Maui.Pages;

public partial class WelcomePage : ContentPage
{
    public WelcomePage(WelcomeModel model)
    {
        InitializeComponent();
        this.WithModel(model);
    }
}