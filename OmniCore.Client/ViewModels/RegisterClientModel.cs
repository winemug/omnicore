using CommunityToolkit.Mvvm.Input;
using OmniCore.Client.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OmniCore.Client.ViewModels;

public class RegisterClientModel : ViewModel
{
    private readonly NavigationModel navigationModel;

    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
    public bool EntryEnabled { get; set; } = true;

    public ICommand CreateAccountCommand => new RelayCommand(CreateAccountClicked);
    public ICommand RegisterClientCommand => new RelayCommand(RegisterClientClicked);

    public RegisterClientModel(NavigationModel navigationModel, RegisterClientPage page) : base(page)
    {
        this.navigationModel = navigationModel;
    }
    private async void CreateAccountClicked()
    {
        EntryEnabled = false;
        OnPropertyChanged();
        await navigationModel.NavigateTo<CreateAccountModel>(false);
    }
    private async void RegisterClientClicked()
    {
        EntryEnabled = false;
        OnPropertyChanged();
    }

    public override ValueTask OnAppear()
    {
        EntryEnabled = true;
        OnPropertyChanged();
        return ValueTask.CompletedTask;
    }
}
