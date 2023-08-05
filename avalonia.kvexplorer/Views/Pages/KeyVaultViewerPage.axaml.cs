using avalonia.kvexplorer.ViewModels;
using Avalonia.Controls;
using Avalonia.Threading;

namespace avalonia.kvexplorer.Views.Pages;

public partial class KeyVaultViewerPage : UserControl
{
    public KeyVaultViewerPage()
    {
        InitializeComponent();

        var keyVaultPageViewModel =new KeyVaultPageViewModel();
        DataContext = keyVaultPageViewModel;

        //Dispatcher.UIThread.Post(async () =>
        //{
        //    await keyVaultPageViewModel.GetAvailableKeyVaultsCommand.ExecuteAsync(null);
        //});
    }
}