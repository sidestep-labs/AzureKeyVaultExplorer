using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using FluentAvalonia.UI.Controls;
using System;

namespace KeyVaultExplorer.ViewModels;

public class NotificationViewModel
{
    public WindowNotificationManager? NotificationManager { get; set; } = new();

    public void AddMessage(Notification notification)
    {
        NotificationManager?.Show(notification);
    }

    public async void ShowErrorPopup(Notification notification)
    {
        // Declaring a TaskDialog from C#:
        var td = new TaskDialog
        {
            // Title property only applies on Windowed dialogs
            Title = notification.Title,
            Header = notification.Title,
            Content = notification.Message,
            //IconSource = new SymbolIconSource { Symbol = Symbol.clos },
            FooterVisibility = TaskDialogFooterVisibility.Auto,
            //Footer = new CheckBox { Content = "Never show me this again" },

            Buttons = { TaskDialogButton.CloseButton }
        };
        td.XamlRoot = App.Current.GetTopLevel();

        var result = await td.ShowAsync();
    }
}