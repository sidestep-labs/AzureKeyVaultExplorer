using avalonia.kvexplorer.ViewModels;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Input;
using Avalonia.Interactivity;
using Azure.Security.KeyVault.Secrets;
using kvexplorer.shared.Models;
using System;
using System.Diagnostics;

namespace avalonia.kvexplorer.Views.Pages;

public partial class VaultPage : UserControl
{
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