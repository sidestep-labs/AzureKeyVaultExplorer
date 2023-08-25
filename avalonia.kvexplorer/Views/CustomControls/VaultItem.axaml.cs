using avalonia.kvexplorer.ViewModels;
using Avalonia.Controls;

namespace avalonia.kvexplorer.Views.CustomControls;

public partial class VaultItem : UserControl
{
    public VaultItem()
    {
        InitializeComponent();

        var keyVaultTreeListViewModel = new VaultItemViewModel();
        DataContext = keyVaultTreeListViewModel;
    }
}