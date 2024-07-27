using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Azure.Security.KeyVault.Secrets;
using FluentAvalonia.UI.Controls;
using FluentAvalonia.UI.Windowing;
using KeyVaultExplorer.Exceptions;
using KeyVaultExplorer.ViewModels;
using KeyVaultExplorer.Views.Pages;
using System.Linq;
using System.Threading.Tasks;

namespace KeyVaultExplorer.Views.CustomControls;

public partial class ToolBar : UserControl
{
    public ToolBar()
    {
        InitializeComponent();
        DataContext = Defaults.Locator.GetRequiredService<ToolBarViewModel>();
    }

    private void SettingsButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Control control = (Control)sender!;
        control.RaiseEvent(new RoutedEventArgs(MainView.NavigateSettingsEvent));
    }

    private void SubscriptionsButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Control control = (Control)sender!;
        control.RaiseEvent(new RoutedEventArgs(MainView.NavigateSubscriptionsEvent));
    }

    private void IsPaneToggledButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Control control = (Control)sender!;
        control.RaiseEvent(new RoutedEventArgs(TabViewPage.PaneToggledRoutedEvent));
    }
}