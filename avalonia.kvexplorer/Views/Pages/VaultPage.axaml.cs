using avalonia.kvexplorer.ViewModels;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Azure.Security.KeyVault.Secrets;
using FluentAvalonia.Core;
using kvexplorer.shared.Models;
using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
namespace avalonia.kvexplorer.Views.Pages;

public partial class VaultPage : UserControl
{
    private DataGrid dataGrid { get; set; }

    public VaultPage()
    {
        InitializeComponent();
        DataContext = new VaultPageViewModel();
        dataGrid = this.FindControl<DataGrid>("VaultContentDataGrid");

    }

    public VaultPage(Uri kvUri)
    {
        InitializeComponent();
        var model = new VaultPageViewModel();
        DataContext = model;
        dataGrid = this.FindControl<DataGrid>("VaultContentDataGrid");

        Dispatcher.UIThread.Post(() =>
        {
            _ = model.GetSecretsForVault(kvUri);
            dataGrid.ItemsSource = new DataGridCollectionView(dataGrid.ItemsSource)
            {
                GroupDescriptions = { new DataGridPathGroupDescription("Type") }
            };
        }, DispatcherPriority.ContextIdle);
    }



    // cruft. Can't figure out a way to regroup from view model.
    private void SearchBoxChanges(object? sender, TextChangedEventArgs e)
    {
        if (dataGrid.ItemsSource.Count() > 0)
        {
            dataGrid.ItemsSource = new DataGridCollectionView(dataGrid.ItemsSource)
            {
                GroupDescriptions = {  new DataGridPathGroupDescription("Type") }
            };
        }
    }



    private void VaultFiltercChanges(object? sender, RoutedEventArgs e)
    {
        if (dataGrid.ItemsSource.Count() > 0)
        {
            dataGrid.ItemsSource = new DataGridCollectionView(dataGrid.ItemsSource)
            {
                GroupDescriptions =
            {
                new DataGridPathGroupDescription("Type")
            }
            };
        }
    }

    private void OnDoubleTapped(object sender, TappedEventArgs e)
    {
        // Do something when double tapped
        var dg = (DataGrid)sender;
        var model = dg.SelectedItem as SecretProperties;
        //Debug.Write(model.Name);
    }
}