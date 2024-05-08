using Avalonia.Controls.Notifications;

namespace KeyVaultExplorer.ViewModels;

public class NotificationViewModel
{
    public WindowNotificationManager? NotificationManager { get; set; } = new();

    public void AddMessage(Notification notification)
    {
        NotificationManager?.Show(notification);
    }
}