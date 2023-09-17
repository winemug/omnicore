﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OmniCore.Client.Interfaces.Services;
using OmniCore.Client.Mobile.Views;

namespace OmniCore.Client.Mobile.ViewModels;
public partial class AccountLoginViewModel : BaseViewModel<int>
{
    private readonly INavigationService _navigationService;

    public AccountLoginViewModel(INavigationService  navigationService)
    {
        _navigationService = navigationService;
    }

    [RelayCommand]
    public async void Continue()
    {
        await _navigationService.PushAsync<PermissionsPage, PermissionsViewModel>();
    }

    public override ValueTask InitializeAsync(int data)
    {
        return ValueTask.CompletedTask;
    }
}