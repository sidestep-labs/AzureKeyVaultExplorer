using avalonia.kvexplorer.ViewModels;
using avalonia.kvexplorer.Views.Pages;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Azure.ResourceManager.KeyVault;
using FluentAvalonia.Core;
using FluentAvalonia.UI.Controls;
using kvexplorer.shared;
using kvexplorer.shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace avalonia.kvexplorer.Views.CustomControls;

public partial class KeyVaultTreeList : UserControl
{
    private readonly TabViewPageViewModel _tabViewViewModel;

    public KeyVaultTreeList()
    {
        InitializeComponent();
        DataContext = Defaults.Locator.GetRequiredService<KeyVaultTreeListViewModel>(); ;
        _tabViewViewModel = Defaults.Locator.GetRequiredService<TabViewPageViewModel>();
        SubscriptionTreeViewList = this.FindControl<TreeView>("SubscriptionTreeViewList");
        SubscriptionTreeViewList.ContextRequested += OnDataGridRowContextRequested;

        // TODO: Figure out why this breaks NativeAOT, possibly due to DI using reflection? idk FIX:
        /* System.TypeInitializationException: A type initializer threw an exception. To determine which type, inspect the InnerException's StackTrace property.
        ---> System.MissingMethodException: No parameterless constructor defined for type 'System.Diagnostics.ActivitySource'.*/
   
    }





    private void OnDataGridRowContextRequested(object sender, ContextRequestedEventArgs e)
    {
        var tv = sender as TreeView;
        if(tv.SelectedItem is not null)
        {
            var kvm = tv.ItemsSource.ElementAt(0) as KeyVaultModel;
            var showUnpin = kvm.KeyVaultResources.Contains(tv.SelectedItem as KeyVaultResource);
            ShowMenu(isTransient: true, isCurrentlyPinned: showUnpin);
        }
        e.Handled = true;
    }

    private void ShowMenu(bool isTransient, bool isCurrentlyPinned)
    {
        var flyout = Resources["FAMenuFlyoutSubscriptionTreeView"] as FAMenuFlyout;

        // if pinned, enable unpinned item
        foreach(MenuFlyoutItem item in flyout.Items)
        {
            _ = item.Name switch
            {
                "PinToQuickAccess" => item.IsVisible = !isCurrentlyPinned,
                "UnpinFromQuickAccess" => item.IsVisible = isCurrentlyPinned,
                _ => item.IsVisible = true
            };
        }
        //var unpinnedItem = (MenuFlyoutItem)flyout.Items.ElementAt(1);
        //unpinnedItem.
        /*
                 <ui:MenuFlyoutItem
                Name="PinToQuickAccess"
                Command="{Binding PinVaultToQuickAccessCommand}"
                CommandParameter="{Binding #SubscriptionTreeViewList.SelectedItem}"
                IconSource="OutlineStar"
                Text="Pin to Quick Access" />*/
        //var unpinItem = new MenuFlyoutItem()
        //{
        //    Name = "UnPinToQuickAccess",
        //    IsEnabled = false,
        //    Text = "Remove From Quick Access",
        //    IconSource = new SymbolIconSource { Symbol = Symbol.StarOff },
        //    Command = (DataContext as KeyVaultTreeListViewModel).PinVaultToQuickAccessCommand
        //};
        //flyout.Items.Add(unpinItem);
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