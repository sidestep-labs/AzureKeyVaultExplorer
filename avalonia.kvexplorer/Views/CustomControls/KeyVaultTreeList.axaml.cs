using avalonia.kvexplorer.ViewModels;
using avalonia.kvexplorer.Views.Pages;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Azure.ResourceManager.KeyVault;
using FluentAvalonia.UI.Controls;
using kvexplorer.shared;
using kvexplorer.shared.Models;
using System;
using System.ComponentModel;
using System.Diagnostics;

namespace avalonia.kvexplorer.Views.CustomControls;

public partial class KeyVaultTreeList : UserControl
{
    private readonly TabViewPageViewModel _tabViewViewModel;

    public KeyVaultTreeList()
    {
        InitializeComponent();

        var model = new KeyVaultTreeListViewModel();
        DataContext = model;
        _tabViewViewModel = Defaults.Locator.GetRequiredService<TabViewPageViewModel>();


        SubscriptionTreeViewList = this.FindControl<TreeView>("SubscriptionTreeViewList");
        SubscriptionTreeViewList.ContextRequested += OnDataGridRowContextRequested;

        // TODO: Figure out why this breaks NativeAOT, possibly due to DI using reflection? idk FIX:
        /* System.TypeInitializationException: A type initializer threw an exception. To determine which type, inspect the InnerException's StackTrace property.
        ---> System.MissingMethodException: No parameterless constructor defined for type 'System.Diagnostics.ActivitySource'.*/
        Dispatcher.UIThread.Post(async () =>
        {
            await model.GetAvailableKeyVaultsCommand.ExecuteAsync(false);
        }, DispatcherPriority.ApplicationIdle);
    }





    private void OnDataGridRowContextRequested(object sender, ContextRequestedEventArgs e)
    {
        var tv = sender as TreeView;
        if(tv.SelectedItem is not null)
            ShowMenu(true);
        e.Handled = true;
    }

    private void ShowMenu(bool isTransient)
    {
        var flyout = Resources["FAMenuFlyoutSubscriptionTreeView"] as FAMenuFlyout;
        flyout.ShowMode = isTransient ? FlyoutShowMode.Transient : FlyoutShowMode.Standard;
        var loc = this.FindControl<TreeView>("SubscriptionTreeViewList");
        flyout.ShowAt(loc);
    }



    private void RefreshKeyVaultList(object sender, RoutedEventArgs e)
    {
        Dispatcher.UIThread.Post(async () =>
        {
            await (DataContext as KeyVaultTreeListViewModel).GetAvailableKeyVaultsCommand.ExecuteAsync(true);
        }, DispatcherPriority.Input);
    }

    private void OnDoubleClicked(object sender, TappedEventArgs args)
    {
        var sx = (TreeView)sender;

        if (sx.SelectedItem is not null)
        {
         
            Dispatcher.UIThread.Post(() =>
            {
                var model = (KeyVaultResource)sx.SelectedItem;

                _tabViewViewModel.AddVaultPageCommand.Execute(model.Data);
            }, DispatcherPriority.ContextIdle);
        }
    }

    private void OnTreeListSelectionChangedTest(object sender, SelectionChangedEventArgs e)
    {
        var s = (TreeView)sender;

        if (s.SelectedItem is not null)
        {
            var model = (KeyVaultModel)s.SelectedItem;
        }
    }
}