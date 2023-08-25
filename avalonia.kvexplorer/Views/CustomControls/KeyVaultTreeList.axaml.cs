using avalonia.kvexplorer.ViewModels;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using System;
using System.ComponentModel;
using System.Diagnostics;

namespace avalonia.kvexplorer.Views.CustomControls;

public partial class KeyVaultTreeList : UserControl
{
    public KeyVaultTreeList()
    {
        InitializeComponent();

        var keyVaultTreeListViewModel = new KeyVaultTreeListViewModel();
        DataContext = keyVaultTreeListViewModel;

        Dispatcher.UIThread.Post(async () =>
        {
            await keyVaultTreeListViewModel.GetAvailableKeyVaultsCommand.ExecuteAsync(null);
        }, DispatcherPriority.Background);
    }

    private void OnTreeListSelectionChangedTest(object sender, SelectionChangedEventArgs e)
    {

     if ((sender as TreeView).SelectedItem is not null)
        {
            Debug.WriteLine(sender.ToString());
        }
    }
    private void PointerPressedx(object sender, PropertyChangedEventArgs e)
    {
        // Handle changes to the SelectedTreeItem property here
        Debug.WriteLine("PointerPressed");
    }
}