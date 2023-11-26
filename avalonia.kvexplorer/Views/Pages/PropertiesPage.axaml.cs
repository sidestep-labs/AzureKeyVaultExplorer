using avalonia.kvexplorer.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace avalonia.kvexplorer;

public partial class PropertiesPage : UserControl
{
    public PropertiesPage()
    {
        InitializeComponent();
        DataContext = new PropertiesPageViewModel();
    }




}