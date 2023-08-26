using avalonia.kvexplorer.ViewModels;
using Avalonia.Controls;

namespace avalonia.kvexplorer.Views.Pages;

public partial class VaultPage : UserControl
{
    public VaultPage()
    {
        InitializeComponent();
        DataContext = new VaultPageViewModel();
    }

}