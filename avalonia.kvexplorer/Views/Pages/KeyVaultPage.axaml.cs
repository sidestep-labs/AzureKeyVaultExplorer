using avalonia.kvexplorer.ViewModels;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Interactivity;
using Avalonia.Media;
using FluentAvalonia.UI.Windowing;
using kvexplorer.shared;

namespace avalonia.kvexplorer.Views.Pages;

public partial class KeyVaultPage : ViewModelBase
{
    private KeyVaultPageViewModel _keyVaultPageViewModel;
    private AuthService _authService;


    public KeyVaultPage()
    {
        InitializeComponent();
    }


    public KeyVaultPage(AuthService authService, KeyVaultPageViewModel keyVaultPageViewModel)
    {
        _authService = authService;
        _keyVaultPageViewModel = keyVaultPageViewModel;
    }

   
}