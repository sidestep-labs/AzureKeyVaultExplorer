using avalonia.kvexplorer.ViewModels;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Threading;
using Azure.Security.KeyVault.Secrets;
using FluentAvalonia.Core;
using System;
using System.Linq;

namespace avalonia.kvexplorer.Views.Pages;

public partial class VaultPage : UserControl
{
    private const string DatGridElementName = "VaultContentDataGrid";
    private readonly VaultPageViewModel vaultPageViewModel;

    public VaultPage()
    {
        InitializeComponent();

        var model = new VaultPageViewModel();
        DataContext = model;
        vaultPageViewModel = model;
        ValuesDataGrid = this.FindControl<DataGrid>(DatGridElementName);
    }

    public VaultPage(Uri kvUri)
    {
        InitializeComponent();
        var model = new VaultPageViewModel();

        DataContext = model;
        vaultPageViewModel = model;
        ValuesDataGrid = this.FindControl<DataGrid>(DatGridElementName);
        Dispatcher.UIThread.Post(() =>
        {
            _ = model.GetSecretsForVault(kvUri);
            ValuesDataGrid.ItemsSource = new DataGridCollectionView(ValuesDataGrid.ItemsSource)
            {
                GroupDescriptions = { new DataGridPathGroupDescription("Type") }
            };
        }, DispatcherPriority.ContextIdle);
    }

    private DataGrid? ValuesDataGrid { get; set; }

    // cruft. Can't figure out a way to regroup from view model.
    private void CheckBox_Click(object sender, RoutedEventArgs e)
    {
        ;
        Dispatcher.UIThread.Invoke(() =>
        {
            vaultPageViewModel.FilterBasedOnCheckedBoxes();
            ValuesDataGrid.ItemsSource = new DataGridCollectionView(ValuesDataGrid.ItemsSource)
            {
                GroupDescriptions = { new DataGridPathGroupDescription("Type") }
            };
        }, DispatcherPriority.Input);
        Dispatcher.UIThread.Invoke(() =>
        {
        }, DispatcherPriority.Input);
    }

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
        if (ValuesDataGrid?.ItemsSource.Count() > 0)
        {
            ValuesDataGrid.ItemsSource = new DataGridCollectionView(ValuesDataGrid.ItemsSource)
            {
                GroupDescriptions = { new DataGridPathGroupDescription("Type") }
            };
        }
    }
}