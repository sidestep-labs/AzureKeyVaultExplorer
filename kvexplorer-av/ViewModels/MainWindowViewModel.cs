using CommunityToolkit.Mvvm.Input;
using kvexplorer_av.Services;
using System.Threading;

namespace kvexplorer_av.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly AuthService _authService;

    public MainWindowViewModel(AuthService authService)
    {
        _authService = authService;
    }

    [RelayCommand]
    private async void Login()
    {
        var cancellation = new CancellationToken();
        var account = await _authService.RefreshTokenAsync(cancellation);
        if(account == null)
        {
            await _authService.LoginAsync(cancellation);
        }

    }

    public string Greeting => "Welcome to Avalonia!";
}