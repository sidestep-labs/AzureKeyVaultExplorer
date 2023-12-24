using Avalonia.Controls;
using kvexplorer.shared;
using kvexplorer.ViewModels;

namespace kvexplorer;

public partial class PropertiesPage : UserControl
{
    private VaultService _vaultService;

    public PropertiesPage()
    {
        InitializeComponent();
        DataContext = new PropertiesPageViewModel();
        _vaultService = Defaults.Locator.GetRequiredService<VaultService>();
    }
}