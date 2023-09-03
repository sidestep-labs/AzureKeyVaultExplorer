using avalonia.kvexplorer.ViewModels;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Azure.Security.KeyVault.Secrets;
using kvexplorer.shared.Models;
using System;
using System.Diagnostics;

namespace avalonia.kvexplorer.Views.Pages;

public partial class VaultPage : UserControl
{
    public VaultPage(Uri kvUri)
    {
        InitializeComponent();
        var model = new VaultPageViewModel();
        Dispatcher.UIThread.Post(() => _ = model.GetSecretsForVault(kvUri), DispatcherPriority.ContextIdle);
        DataContext = model;
    }
    public VaultPage()
    {
        InitializeComponent();
        DataContext = new VaultPageViewModel();
    }
    private void OnDoubleTapped(object sender, TappedEventArgs e)
    {
        // Do something when double tapped
        var dg = (DataGrid)sender;
        var model = dg.SelectedItem as SecretProperties;
        //Debug.Write(model.Name);
    }
}