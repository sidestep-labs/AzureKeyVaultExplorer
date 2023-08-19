using avalonia.kvexplorer.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using System;
using System.ComponentModel;

namespace avalonia.kvexplorer.Views.Pages;

public partial class KeyVaultViewerPage : UserControl
{
    public KeyVaultViewerPage()
    {
        InitializeComponent();

        var keyVaultPageViewModel = new KeyVaultPageViewModel();
        DataContext = keyVaultPageViewModel;

        Dispatcher.UIThread.Post(async () =>
        {
            await keyVaultPageViewModel.GetAvailableKeyVaultsCommand.ExecuteAsync(null);
        }, DispatcherPriority.Background);
    }

    private void OnTreeListSelectionChangedTest(object sender, SelectionChangedEventArgs e)
    {
        if ((sender as TreeView).SelectedItem is not null)
        {
            Console.WriteLine(sender.ToString());
        }
    }
}