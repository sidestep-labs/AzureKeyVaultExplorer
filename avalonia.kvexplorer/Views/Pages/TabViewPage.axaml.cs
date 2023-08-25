using avalonia.kvexplorer.ViewModels;
using avalonia.kvexplorer.Views.CustomControls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using FluentAvalonia.Core;
using FluentAvalonia.UI.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using static avalonia.kvexplorer.ViewModels.TabViewPageViewModel;

namespace avalonia.kvexplorer.Views.Pages;

public partial class TabViewPage : UserControl
{
    public TabViewPage()
    {
        InitializeComponent();

        var vm = new TabViewPageViewModel();
        DataContext = vm;

       
    }

    private void TabView_AddButtonClick(TabView sender, EventArgs args)
    {
        (sender.TabItems as IList).Add(CreateNewTab(sender.TabItems.Count()));
    }

    private void TabView_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
    {
        (sender.TabItems as IList).Remove(args.Tab);
    }

    private TabViewItem CreateNewTab(int index)
    {
        var tvi = new TabViewItem
        {
            Header = $"Vault Item {index}",
            IconSource = new SymbolIconSource { Symbol = Symbol.Document }
        };

        switch (index % 3)
        {
            case 0:
                tvi.Content = new VaultItem();
                break;

            case 1:
                tvi.Content = new VaultItem();
                break;

            case 2:
                tvi.Content = new VaultItem();
                break;
        }

        return tvi;
    }

    private void BindingTabView_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
    {
        (DataContext as TabViewPageViewModel).Documents.Remove(args.Item as DocumentItem);
    }

}