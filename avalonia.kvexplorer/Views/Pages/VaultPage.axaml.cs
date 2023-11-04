﻿using avalonia.kvexplorer.ViewModels;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Notifications;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Styling;
using Avalonia.Threading;
using Azure.Security.KeyVault.Secrets;
using FluentAvalonia.Core;
using FluentAvalonia.UI.Controls;
using kvexplorer.shared.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Channels;
using Avalonia.Platform;

using System.IO;

using System.Reflection.Metadata;
using kvexplorer.shared;
using CommunityToolkit.Mvvm.Input;
using kvexplorer.shared.Exceptions;
using System.Threading.Tasks;

namespace avalonia.kvexplorer.Views.Pages;

public partial class VaultPage : UserControl
{
    private const string DatGridElementName = "VaultContentDataGrid";
    private readonly VaultPageViewModel vaultPageViewModel;
    private readonly INotificationManager _notificationManager;

    public VaultPage()
    {
        InitializeComponent();
        var model = new VaultPageViewModel();
        DataContext = model;
        vaultPageViewModel = model;
        ValuesDataGrid = this.FindControl<DataGrid>(DatGridElementName);
        ValuesDataGrid.ContextRequested += OnDataGridRowContextRequested;

        Dispatcher.UIThread.Post(() =>
        {
            ValuesDataGrid.ItemsSource = new DataGridCollectionView(ValuesDataGrid.ItemsSource)
            {
                GroupDescriptions = { new DataGridPathGroupDescription("Type") }
            };
        }, DispatcherPriority.ContextIdle);
    }

    public VaultPage(Uri kvUri)
    {
        InitializeComponent();
        var model = new VaultPageViewModel();
        DataContext = model;
        vaultPageViewModel = model;
        ValuesDataGrid = this.FindControl<DataGrid>(DatGridElementName);
        ValuesDataGrid.ContextRequested += OnDataGridRowContextRequested;
        var copyItemToClipboard = this.FindControl<MenuFlyoutItem>("CopyMenuFlyoutItem");

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

    // We rely on code behind to show the flyout

    // Listen for the ContextRequested event so we can change the launch behavior based on whether it was a
    // left or right click.

    private void OnDataGridRowContextRequested(object sender, ContextRequestedEventArgs e)
    {
        ShowMenu(true);
        e.Handled = true;
    }

    private void ShowMenu(bool isTransient)
    {
        var flyout = Resources["FAMenuFlyout"] as FAMenuFlyout;
        flyout.ShowMode = isTransient ? FlyoutShowMode.Transient : FlyoutShowMode.Standard;
        flyout.ShowAt(this.FindControl<DataGrid>(DatGridElementName));
    }

    public void button_Click(object sender, RoutedEventArgs e)
    {
        // Change button text when button is clicked.
        var not = new Notification("Test", "Button Clicked", NotificationType.Information);
        var nm = new WindowNotificationManager((Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime).MainWindow)
        {
            Position = NotificationPosition.BottomRight,
            MaxItems = 1,
        };

        var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
        Notification notif;
        var dataObject = new DataObject();
        dataObject.Set(DataFormats.Text, "TEst arthur");
        clipboard.SetTextAsync("sdfdjkfhsdkjfhd");

        nm.TemplateApplied += (sender, args) =>
               {
                   nm.Show(not);
               };
    }

    protected void DataGrid_CopyingRowClipboardContent(object sender, DataGridRowClipboardEventArgs e)
    {
        Dispatcher.UIThread.InvokeAsync(async () =>
        {
            await vaultPageViewModel.CopyCommand.ExecuteAsync((KeyVaultContentsAmalgamation)e.Item);
        });
    }
}