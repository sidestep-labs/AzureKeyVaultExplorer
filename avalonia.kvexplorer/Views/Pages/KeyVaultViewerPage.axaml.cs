using avalonia.kvexplorer.ViewModels;
using Avalonia.Controls;

namespace avalonia.kvexplorer.Views.Pages;

public partial class KeyVaultViewerPage : UserControl
{
    public KeyVaultViewerPage()
    {
        InitializeComponent();
        DataContext = new KeyVaultPageViewModel();
    }
}