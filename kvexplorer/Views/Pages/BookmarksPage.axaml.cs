using kvexplorer.ViewModels;
using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Messaging;
using System.Diagnostics;
using Avalonia.Controls.ApplicationLifetimes;
using FluentAvalonia.UI.Controls.Experimental;
using FluentAvalonia.UI.Controls;
using FluentAvalonia.UI.Navigation;
using Avalonia.Interactivity;
using System;

namespace kvexplorer.Views.Pages;

public partial class BookmarksPage : UserControl
{
    private bool IsInitialLoad = true;

    public BookmarksPage()
    {
        InitializeComponent();
        DataContext = new BookmarksPageViewModel();
        AddHandler(Frame.NavigatedToEvent, OnNavigatedTo, RoutingStrategies.Direct);
    }

    private void OnNavigatedTo(object sender, NavigationEventArgs e)
    {
        if (e.NavigationMode != NavigationMode.Back && IsInitialLoad)
        {
            IsInitialLoad = false;

        Dispatcher.UIThread.InvokeAsync(async () =>
        {
            await (DataContext as BookmarksPageViewModel)!.GetAllKeyVaults();
        }, DispatcherPriority.Background);
        }
    }
}