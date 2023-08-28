using avalonia.kvexplorer.ViewModels;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Azure.ResourceManager.KeyVault;
using kvexplorer.shared.Models;
using System;
using System.ComponentModel;
using System.Diagnostics;

namespace avalonia.kvexplorer.Views.CustomControls;

public partial class KeyVaultTreeList : UserControl
{

    public KeyVaultTreeList()
    {
        InitializeComponent();

        var model = new KeyVaultTreeListViewModel();
        DataContext = model;

        Dispatcher.UIThread.Post(async () =>
        {
            await model.GetAvailableKeyVaultsCommand.ExecuteAsync(null);
        }, DispatcherPriority.Background);
    }

    private void OnTreeListSelectionChangedTest(object sender, SelectionChangedEventArgs e)
    {
        var s = (TreeView)sender;

        if (s.SelectedItem is not null)
        {
            var model = (KeyVaultModel)s.SelectedItem;
        }
    }

    private void PointerPressedx(object sender, PropertyChangedEventArgs e)
    {
        // Handle changes to the SelectedTreeItem property here
        Debug.WriteLine("PointerPressed");
    }
}