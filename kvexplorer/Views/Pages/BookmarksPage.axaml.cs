using kvexplorer.ViewModels;
using Avalonia.Controls;
using System;

#if WINDOWS
using NotificationHelper;
using System.Runtime.InteropServices;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
#endif

namespace kvexplorer.Views.Pages;

public partial class BookmarksPage : UserControl
{
    public BookmarksPage()
    {
        InitializeComponent();
        DataContext = new BookmarksPageViewModel();
    }

    private void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {



#if WINDOWS
    var toastNotifier = ToastNotificationManagerCompat.CreateToastNotifier();

        string toastXml = @"<toast><visual><binding template='ToastText01'><text id='1'>Hello, World!</text></binding></visual></toast>";

        XmlDocument doc = new XmlDocument();
        doc.LoadXml(toastXml);
        ToastNotification toast = new ToastNotification(doc);
        toast.ExpirationTime = DateTimeOffset.Now + TimeSpan.FromSeconds(10);
        toastNotifier.Show(toast);

        Console.WriteLine("Press any key to exit...");
#endif
    }
}