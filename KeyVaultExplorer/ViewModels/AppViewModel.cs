using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KeyVaultExplorer.Views;
using KeyVaultExplorer.Services;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;

namespace KeyVaultExplorer.ViewModels;

public partial class AppViewModel : ViewModelBase
{

    // private readonly AuthService _authService;

    [ObservableProperty]
    public string email;


    [ObservableProperty]
    public bool isAuthenticated = false;
    public AppViewModel()
    {
        // _authService = Defaults.Locator.GetRequiredService<AuthService>();

    }

    [RelayCommand]
    public void About()
    {
        var aboutWindow = new AboutPageWindow()
        {
            Title = "About Key Vault Explorer",
            Width = 380,
            Height = 200,
            CanResize = false,
            SizeToContent = SizeToContent.Manual,
            WindowStartupLocation = WindowStartupLocation.Manual,
        };

        var top = Avalonia.Application.Current.GetTopLevel() as MainWindow;
        aboutWindow.ShowDialog(top);
    }

  
}