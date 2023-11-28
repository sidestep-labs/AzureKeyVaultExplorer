using kvexplorer.ViewModels;
using Avalonia.Controls;
using Microsoft.Toolkit.Uwp.Notifications;
using System.Runtime.InteropServices;
using System.Reflection.Emit;
using System;

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
        var toast = new ToastContentBuilder()
             .AddArgument("action", "viewConversation")
             .AddArgument("conversationId", 9813)
             .AddText("Andrew sent you a picture")
             .AddText("Check this out, The Enchantments in Washington!");


#if WINDOWS
            toast.Show();
#endif

    }
}