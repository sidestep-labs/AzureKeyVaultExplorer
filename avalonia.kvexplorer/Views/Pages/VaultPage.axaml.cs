using avalonia.kvexplorer.ViewModels;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Azure.Security.KeyVault.Secrets;
using FluentAvalonia.Core;
using System;
using System.Linq;

namespace avalonia.kvexplorer.Views.Pages;

public partial class VaultPage : UserControl
{
    private readonly VaultPageViewModel vaultPageViewModel;
    public VaultPage()
    {
        InitializeComponent();
        var model = new VaultPageViewModel();
        DataContext = model;
        vaultPageViewModel = model;
        dataGrid = this.FindControl<DataGrid>("VaultContentDataGrid");
    }

    public VaultPage(Uri kvUri)
    {
        InitializeComponent();
        var model = new VaultPageViewModel();
        DataContext = model;
        vaultPageViewModel = model;
        dataGrid = this.FindControl<DataGrid>("VaultContentDataGrid");

        Dispatcher.UIThread.Post(() =>
        {
            _ = model.GetSecretsForVault(kvUri);
            dataGrid.ItemsSource = new DataGridCollectionView(dataGrid.ItemsSource)
            {
                GroupDescriptions = { new DataGridPathGroupDescription("Type") }
            };
            (DataContext as VaultPageViewModel).IsBusy = false;
        }, DispatcherPriority.ContextIdle);
    }

    private DataGrid? dataGrid { get; set; }
    private bool IsBusyFiltering { get; set; } = false;
    private void OnDoubleTapped(object sender, TappedEventArgs e)
    {
        // Do something when double tapped
        var dg = (DataGrid)sender;
        var model = dg.SelectedItem as SecretProperties;
        //Debug.Write(model.Name);
    }

    // cruft. Can't figure out a way to regroup from view model.
    private void SearchBoxChanges(object? sender, TextChangedEventArgs e)
    {
        if (dataGrid?.ItemsSource.Count() > 0)
        {
            dataGrid.ItemsSource = new DataGridCollectionView(dataGrid.ItemsSource)
            {
                GroupDescriptions = { new DataGridPathGroupDescription("Type") }
            };
        }
    }

    private void VaultFilterChanges(object? sender, RoutedEventArgs e)
    {
        if (IsBusyFiltering)
            return;

        Dispatcher.UIThread.Post(() =>
        {
            vaultPageViewModel.FilterBasedOnCheckedBoxes();

            if (dataGrid?.ItemsSource.Count() > 0)
            {
                IsBusyFiltering = true;
                dataGrid.ItemsSource = new DataGridCollectionView(dataGrid.ItemsSource)
                {
                    GroupDescriptions = { new DataGridPathGroupDescription("Type") }
                };
                IsBusyFiltering = false;
            }
        }, DispatcherPriority.Input);
    }
}