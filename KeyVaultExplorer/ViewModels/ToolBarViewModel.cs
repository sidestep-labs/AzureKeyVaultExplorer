using CommunityToolkit.Mvvm.ComponentModel;

namespace KeyVaultExplorer.ViewModels;

public partial class ToolBarViewModel : ViewModelBase
{
    [ObservableProperty]
    public bool isBusy;

    public ToolBarViewModel()
    {
    }
}