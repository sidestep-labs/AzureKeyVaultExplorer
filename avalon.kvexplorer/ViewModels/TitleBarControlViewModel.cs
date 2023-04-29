using avalon.kvexplorer.Models;
using avalon.kvexplorer.Services;
using Azure.ResourceManager.KeyVault;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace avalon.kvexplorer.ViewModels;

public partial class TitleBarViewModel : ViewModelBase
{
    private readonly AuthService _authService;

    public string Greeting => "Welcome to Avalonia!";

    public TitleBarViewModel(AuthService authService, VaultService vaultService)
    {
        _authService = authService;
    }

    public TitleBarViewModel()
    {
        _authService = new AuthService();
    }

    [RelayCommand]
    private async void Login()
    {
        var cancellation = new CancellationToken();
        var account = await _authService.RefreshTokenAsync(cancellation);
        if (account == null)
            await _authService.LoginAsync(cancellation);
    }

}