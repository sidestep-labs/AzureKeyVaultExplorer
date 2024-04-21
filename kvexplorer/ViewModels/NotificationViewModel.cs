using Avalonia.Controls.Notifications;

namespace kvexplorer.ViewModels;

public class NotificationViewModel
{
    public WindowNotificationManager? NotificationManager { get; set; } = new();
    public void AddMessage(Notification notification)
    {
        NotificationManager?.Show(notification);
    }
}