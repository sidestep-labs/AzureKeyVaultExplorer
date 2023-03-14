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
        await _authService.LoginAsync(CancellationToken.None);
    }

    public string Greeting => "Welcome to Avalonia!";
}