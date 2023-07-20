using avalonia.kvexplorer.ViewModels;
using Avalonia.Controls;
using kvexplorer.shared;

namespace avalonia.kvexplorer.Views.Pages;

public partial class SettingsPage : UserControl
{
    public SettingsPage()
    {
        InitializeComponent();
        //DataContext = new SettingsPageViewModel();
        DataContext = Defaults.Locator.GetRequiredService<SettingsPageViewModel>();
       
    }
}

