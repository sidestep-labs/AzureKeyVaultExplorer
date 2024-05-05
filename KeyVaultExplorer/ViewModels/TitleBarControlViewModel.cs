using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KeyVaultExplorer.Services;
using System.Threading;

namespace KeyVaultExplorer.ViewModels;

public partial class TitleBarViewModel : ViewModelBase
{
    private readonly AuthService _authService;

    public TitleBarViewModel(AuthService authService, VaultService vaultService)
    {
        _authService = authService;
    }

    [ObservableProperty]
    public string title = "Key Vault Explorer for Azure";

    public TitleBarViewModel()
    {
        _authService = new AuthService();
    }

    [RelayCommand]
    private async void Signin()
    {
        var cancellation = new CancellationToken();
        var account = await _authService.RefreshTokenAsync(cancellation);
        if (account == null)
            await _authService.LoginAsync(cancellation);
    }

    [RelayCommand]
    private async void Signout()
    {
        await _authService.RemoveAccount();
    }
}