using Avalonia.Controls;
using kvexplorer.shared;
using kvexplorer.ViewModels;

namespace kvexplorer;

public partial class PropertiesPage : UserControl
{

    public PropertiesPage()
    {
        InitializeComponent();
        DataContext = new PropertiesPageViewModel();
    }
}