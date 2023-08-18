using avalonia.kvexplorer.ViewModels;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using System.ComponentModel;

namespace avalonia.kvexplorer.Views.CustomControls;

public partial class KeyVaultTreeList : UserControl
{
    public KeyVaultTreeList()
    {
        InitializeComponent();

        var keyVaultPageViewModel = new KeyVaultPageViewModel();
        DataContext = keyVaultPageViewModel;

        Dispatcher.UIThread.Post(async () =>
        {
            await keyVaultPageViewModel.GetAvailableKeyVaultsCommand.ExecuteAsync(null);
        }, DispatcherPriority.Background);
    }

    private void OpenWindowButton_Click(object? sender, RoutedEventArgs e)
    {
        System.Console.WriteLine("PointerPressed");
    }

    private void PointerPressedx(object sender, PropertyChangedEventArgs e)
    {
        // Handle changes to the SelectedTreeItem property here
        System.Console.WriteLine("PointerPressed");
    }
}