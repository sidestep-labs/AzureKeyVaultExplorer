using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using kvexplorer.shared;
using System.Collections.Generic;
using System.Threading;
using System;
using System.Threading.Tasks;

namespace avalonia.kvexplorer.ViewModels;

public partial class SettingsPageViewModel : ViewModelBase
{
    private readonly AuthService _authService;
    public SettingsPageViewModel(AuthService authService)
    {
        _authService = authService;
    }

    public SettingsPageViewModel()
    {
        _authService = new AuthService();
    }



    [RelayCommand]
    private async Task SignIn()
    {
        var cancellation = new CancellationToken();
        var account = await _authService.RefreshTokenAsync(cancellation);
        if (account == null)
            await _authService.LoginAsync(cancellation);
    }

    [RelayCommand]
    private async Task SignOut()
    {
        await _authService.RemoveAccount();
    }

}