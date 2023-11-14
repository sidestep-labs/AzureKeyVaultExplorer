using avalonia.kvexplorer.ViewModels;
using Avalonia.Controls;
using Avalonia.Threading;
using FluentAvalonia.UI.Controls;

namespace avalonia.kvexplorer.Views.Pages;

public partial class SettingsPage : UserControl
{
    public SettingsPage()
    {
        InitializeComponent();
        //DataContext = new SettingsPageViewModel();
        DataContext = Defaults.Locator.GetRequiredService<SettingsPageViewModel>();

    
    }

    private void FetchUserInfoSettingsExpanderItem_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        (DataContext as SettingsPageViewModel).SignInOrRefreshTokenCommand.Execute(null);
    }
}