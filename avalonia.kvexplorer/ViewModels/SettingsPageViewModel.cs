using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using kvexplorer.shared;
using kvexplorer.shared.Models;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace avalonia.kvexplorer.ViewModels;

public partial class SettingsPageViewModel : ViewModelBase
{
    private readonly AuthService _authService;
    private readonly VaultService _vaultService;

    [ObservableProperty]
    private AuthenticatedUserClaims? authenticatedUserClaims;

    public SettingsPageViewModel()
    {
        _authService = Defaults.Locator.GetRequiredService<AuthService>();
        _vaultService = Defaults.Locator.GetRequiredService<VaultService>();
    }

    [RelayCommand]
    private async Task SignInOrRefreshTokenAsync()
    {
        var cancellation = new CancellationToken();
        var account = await _authService.RefreshTokenAsync(cancellation);

        if (account is null)
        {
            account = await _authService.LoginAsync(cancellation);
        }

        AuthenticatedUserClaims = new AuthenticatedUserClaims()
        {
            Username = account.Account.Username,
            TenantId = account.TenantId,
            Name = account.ClaimsPrincipal.Identities.First().FindFirst("name").Value,
            Email = account.ClaimsPrincipal.Identities.First().FindFirst("email").Value,
        };
    }

    [RelayCommand]
    private async Task SignOut()
    {
        await _authService.RemoveAccount();
        AuthenticatedUserClaims = null;
    }
}