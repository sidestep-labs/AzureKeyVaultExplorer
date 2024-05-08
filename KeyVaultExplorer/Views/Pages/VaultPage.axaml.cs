using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using FluentAvalonia.Core;
using FluentAvalonia.UI.Controls;
using KeyVaultExplorer.Models;
using KeyVaultExplorer.ViewModels;
using System;
using System.Linq;

#nullable disable

namespace KeyVaultExplorer.Views.Pages;

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
        ValuesDataGrid.ContextRequested += OnDataGridRowContextRequested;
        KeyUp += Control_KeyUp;
        TabHost.SelectionChanged += TabHostSelectionChanged;
        TabHostSelectionChanged(KeyVaultItemType.Secret, null);
    }

    public VaultPage(Uri kvUri)
    {
        InitializeComponent();
        var model = new VaultPageViewModel();
        DataContext = model;
        vaultPageViewModel = model;
        model.VaultUri = kvUri;
        ValuesDataGrid = this.FindControl<DataGrid>(DatGridElementName);
        ValuesDataGrid.ContextRequested += OnDataGridRowContextRequested;
        var copyItemToClipboard = this.FindControl<MenuFlyoutItem>("CopyMenuFlyoutItem");
        KeyUp += Control_KeyUp;
        TabHost.SelectionChanged += TabHostSelectionChanged;
        TabHostSelectionChanged(KeyVaultItemType.Secret, null);
    }

    private DataGrid ValuesDataGrid { get; set; }

    protected void DataGrid_CopyingRowClipboardContent(object sender, DataGridRowClipboardEventArgs e)
    {
        Dispatcher.UIThread.InvokeAsync(async () =>
        {
            await vaultPageViewModel.CopyCommand.ExecuteAsync((KeyVaultContentsAmalgamation)e.Item);
        });
    }

    private void Control_KeyUp(object sender, KeyEventArgs e)
    {
        if (e.Key == Avalonia.Input.Key.F && (e.KeyModifiers == KeyModifiers.Control || e.Key == Avalonia.Input.Key.LWin || e.Key == Avalonia.Input.Key.RWin))
        {
            SearchTextBox.Focus();
            e.Handled = true;
        }
        if (e.Key == Avalonia.Input.Key.F5)
        {
            Dispatcher.UIThread.Post(async () =>
            {
                await vaultPageViewModel.RefreshCommand.ExecuteAsync(null);
                if (TabHost.SelectedIndex > 2)
                {
                    ValuesDataGrid.ItemsSource = new DataGridCollectionView(ValuesDataGrid.ItemsSource)
                    {
                        GroupDescriptions = { new DataGridPathGroupDescription("Type") }
                    };
                }
            }, DispatcherPriority.Background);
            e.Handled = true;
        }
    }

    private void OnDataGridRowContextRequested(object sender, ContextRequestedEventArgs e)
    {
        var dg = sender as DataGrid;
        var hideCopyCmd = false;
        //if (dg.SelectedItem is not null && (dg.SelectedItem as KeyVaultContentsAmalgamation).Type == KeyVaultItemType.Certificate)
        if (dg.SelectedItem is KeyVaultContentsAmalgamation { Type: KeyVaultItemType.Certificate })
        {
            hideCopyCmd = true;
        }
        ShowMenu(isTransient: true, hideCopyCommand: hideCopyCmd);
        e.Handled = true;
    }

    private void OnDoubleTapped(object sender, TappedEventArgs e)
    {
        // Do something when double tapped
        var dg = (DataGrid)sender;
        var model = dg.SelectedItem as KeyVaultContentsAmalgamation;
        (DataContext as VaultPageViewModel).ShowPropertiesCommand.Execute(model);
        //Debug.Write(model.Name);
    }

    public void RefreshButton_OnClick(object sender, RoutedEventArgs args)
    {
        Dispatcher.UIThread.Post(async () =>
        {
            await vaultPageViewModel.RefreshCommand.ExecuteAsync(null);
            if (TabHost.SelectedIndex > 2)
            {
                ValuesDataGrid.ItemsSource = new DataGridCollectionView(ValuesDataGrid.ItemsSource)
                {
                    GroupDescriptions = { new DataGridPathGroupDescription("Type") }
                };
            }
        }, DispatcherPriority.MaxValue);
    }

    private void SearchBoxChanges(object sender, TextChangedEventArgs e)
    {
        if (TabHost.SelectedIndex > 2)
        {
            ValuesDataGrid.ItemsSource = new DataGridCollectionView(ValuesDataGrid.ItemsSource)
            {
                GroupDescriptions = { new DataGridPathGroupDescription("Type") }
            };
        }
    }

    // We rely on code behind to show the flyout
    // Listen for the ContextRequested event so we can change the launch behavior based on whether it was a
    // left or right click.
    private void ShowMenu(bool isTransient, bool hideCopyCommand)
    {
        var flyout = Resources["FAMenuFlyout"] as FAMenuFlyout;

        // hide the copy value command for the certificate option.
        MenuFlyoutItem copyMenuItemOption = (MenuFlyoutItem)flyout.Items.ElementAt(0);
        copyMenuItemOption.IsVisible = !hideCopyCommand;

        flyout.ShowMode = isTransient ? FlyoutShowMode.Transient : FlyoutShowMode.Standard;
        flyout.ShowAt(this.FindControl<DataGrid>(DatGridElementName));
    }

    private async void TabHostSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var vm = (DataContext as VaultPageViewModel);
        if (vm.VaultUri is null) return;
        var item = TabHost.SelectedIndex switch
        {
            0 => KeyVaultItemType.Secret,
            1 => KeyVaultItemType.Certificate,
            2 => KeyVaultItemType.Key,
            _ => KeyVaultItemType.All
        };

        await vm.FilterAndLoadVaultValueType(item);

        if (item == KeyVaultItemType.All)
        {
            ValuesDataGrid.ItemsSource = new DataGridCollectionView(ValuesDataGrid.ItemsSource)
            {
                GroupDescriptions = { new DataGridPathGroupDescription("Type") }
            };
        }
    }
}