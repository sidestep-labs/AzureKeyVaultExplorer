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
    private readonly NotificationViewModel _notificationViewModel;

    public ToolBar()
    {
        InitializeComponent();
        DataContext = Defaults.Locator.GetRequiredService<ToolBarViewModel>();
        _notificationViewModel = Defaults.Locator.GetRequiredService<NotificationViewModel>();
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

    private void CreateSecret_Clicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Dispatcher.UIThread.Post(() => CreateNewSecret());
    }

    private async Task CreateNewSecret()
    {
        try
        {
            var lifetime = App.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
            var applyChangesBtn = new TaskDialogButton("Apply Changes", "ApplyChangesButtonResult");
            var dialog = new TaskDialog()
            {
                Title = "New Secret",
                XamlRoot = lifetime.Windows.Last() as AppWindow,
                Buttons = { applyChangesBtn, TaskDialogButton.CancelButton, },
                MinWidth = 600,
          
                Content = new CreateNewSecretVersion()
                {
                    DataContext = new CreateNewSecretVersionViewModel()
                    {
                        KeyVaultSecretModel = new SecretProperties("new_secret_1"),
                        IsEdit = false,
                        IsNew = true,
                    }
                }
            };

            applyChangesBtn.Click += async (sender, args) =>
            {
                await (DataContext as CreateNewSecretVersionViewModel)!.NewVersionCommand.ExecuteAsync(null);
            };

            var result = await dialog.ShowAsync();
        }
        catch (KeyVaultItemNotFoundException ex)
        {
        }
        catch (KeyVaultInSufficientPrivileges ex)
        {
            _notificationViewModel.ShowErrorPopup(new Avalonia.Controls.Notifications.Notification { Message = ex.Message, Title = "Insufficient Rights" });
        }
    }
}