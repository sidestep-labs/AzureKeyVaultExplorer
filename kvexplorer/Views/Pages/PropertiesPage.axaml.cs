using kvexplorer.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace kvexplorer;

public partial class PropertiesPage : UserControl
{
    public PropertiesPage()
    {
        InitializeComponent();
        DataContext = new PropertiesPageViewModel();
    }




}