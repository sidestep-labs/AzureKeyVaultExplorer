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
using kvexplorer.shared;

namespace kvexplorer.Views.Pages;

public partial class SubscriptionsPage : UserControl
{
    private bool IsInitialLoad = true;

    public SubscriptionsPage()
    {
        InitializeComponent();
        DataContext = new SubscriptionsPageViewModel();
        AddHandler(Frame.NavigatedToEvent, OnNavigatedTo, RoutingStrategies.Direct);
    }

    private void OnNavigatedTo(object sender, NavigationEventArgs e)
    {
        if (e.NavigationMode != NavigationMode.Back && IsInitialLoad)
        {
            IsInitialLoad = false;

        Dispatcher.UIThread.InvokeAsync(async () =>
        {
            await (DataContext as SubscriptionsPageViewModel)!.GetAllKeyVaults();
        }, DispatcherPriority.Background);
        }
    }

    private async void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {

        var x = Defaults.Locator.GetRequiredService<VaultService>();
        await x.GetStoredSelectedVaults();
    }
}