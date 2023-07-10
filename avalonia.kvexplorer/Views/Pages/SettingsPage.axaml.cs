using avalonia.kvexplorer.ViewModels;
using Avalonia.Controls;

namespace avalonia.kvexplorer.Views.Pages;

public partial class SettingsPage : UserControl
{

    private readonly SettingsPageViewModel _settingsPageViewModel;

    public SettingsPage(SettingsPageViewModel settingsPageViewModel)
    {
        InitializeComponent();
       // _settingsPageViewModel = settingsPageViewModel;
    }

    public SettingsPage()
    {
       //// _settingsPageViewModel = new SettingsPageViewModel();
    }
}

