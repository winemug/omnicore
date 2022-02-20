using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmniCore.Mobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace OmniCore.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SetupAppPage : ContentPage
    {
        public SetupAppPage()
        {
            InitializeComponent();
            new SetupAppViewModel(this);
        }
    }
}