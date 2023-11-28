using kvexplorer.ViewModels;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Notifications;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Azure.Security.KeyVault.Secrets;
using FluentAvalonia.Core;
using FluentAvalonia.UI.Controls;
using kvexplorer.shared.Models;
using System;
using System.IO;
using System.Linq;
using FluentAvalonia.UI.Windowing;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;

#nullable disable

namespace kvexplorer.Views.Pages;

public partial class VaultPage : UserControl
{
    private const string DatGridElementName = "VaultContentDataGrid";
    private readonly INotificationManager _notificationManager;
    private readonly VaultPageViewModel vaultPageViewModel;

    public VaultPage()
    {
        InitializeComponent();
        var model = new VaultPageViewModel();
        DataContext = model;
        vaultPageViewModel = model;
        ValuesDataGrid = this.FindControl<DataGrid>(DatGridElementName);
        ValuesDataGrid.ContextRequested += OnDataGridRowContextRequested;
        KeyUp += MyUserControl_KeyUp;
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
        KeyUp += MyUserControl_KeyUp;
        TabHost.SelectionChanged += TabHostSelectionChanged;
        TabHostSelectionChanged(KeyVaultItemType.Secret, null);
    }

    private DataGrid? ValuesDataGrid { get; set; }

    protected void DataGrid_CopyingRowClipboardContent(object sender, DataGridRowClipboardEventArgs e)
    {
        Dispatcher.UIThread.InvokeAsync(async () =>
        {
            await vaultPageViewModel.CopyCommand.ExecuteAsync((KeyVaultContentsAmalgamation)e.Item);
        });
    }

    private void MyUserControl_KeyUp(object sender, KeyEventArgs e)
    {
        if (e.Key == Avalonia.Input.Key.F && (e.KeyModifiers == KeyModifiers.Control || e.Key == Avalonia.Input.Key.LWin || e.Key == Avalonia.Input.Key.RWin))
        {
            SearchTextBox.Focus();
            e.Handled = true;
        }
    }

    private void OnDataGridRowContextRequested(object sender, ContextRequestedEventArgs e)
    {
        ShowMenu(true);
        e.Handled = true;
    }

    private void OnDoubleTapped(object sender, TappedEventArgs e)
    {
        // Do something when double tapped
        var dg = (DataGrid)sender;
        var model = dg.SelectedItem as SecretProperties;
        //Debug.Write(model.Name);
    }

    private void OpenWindowButton_Click(object? sender, RoutedEventArgs e)
    {
        // Create the window object
        var sampleWindow =
            new Window
            {
                Title = "Sample Window",
                Width = 200,
                Height = 200
            };

        // open the window
        sampleWindow.Show();
    }

    public void RefreshButton_OnClick(object? sender, RoutedEventArgs args)
    {
        Dispatcher.UIThread.Post(async () =>
        {
            await vaultPageViewModel.RefreshCommand.ExecuteAsync(null);
            if (ValuesDataGrid?.ItemsSource.Count() > 0 && !(new int[] { 0, 1, 2 }.Contains(TabHost.SelectedIndex)))
            {
                ValuesDataGrid.ItemsSource = new DataGridCollectionView(ValuesDataGrid.ItemsSource)
                {
                    GroupDescriptions = { new DataGridPathGroupDescription("Type") }
                };
            }
        }, DispatcherPriority.Input);
    }

    private void SearchBoxChanges(object? sender, TextChangedEventArgs e)
    {
        if (ValuesDataGrid?.ItemsSource.Count() > 0 && !(new int[] { 0, 1, 2 }.Contains(TabHost.SelectedIndex)))
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
    private void ShowMenu(bool isTransient)
    {
        var flyout = Resources["FAMenuFlyout"] as FAMenuFlyout;
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
        }; ;
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