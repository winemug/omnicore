using CommunityToolkit.Mvvm.Input;
using OmniCore.Client.Services;
using OmniCore.Client.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OmniCore.Client.ViewModels;

public class AuthenticationModel(NavigationService navigationService) : ViewModel
{
    public string Email { get; set; }
    public string Password { get; set; }
    public ICommand LoginCommand => new RelayCommand(Login);

    public async void Login()
    {
        await navigationService.PushViewAsync<EmptyPage>();
    }

}
