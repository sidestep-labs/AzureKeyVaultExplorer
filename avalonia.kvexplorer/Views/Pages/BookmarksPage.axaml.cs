using avalonia.kvexplorer.ViewModels;
using Avalonia.Controls;

namespace avalonia.kvexplorer.Views.Pages;

public partial class BookmarksPage : UserControl
{
    public BookmarksPage()
    {
        InitializeComponent();
        DataContext = new BookmarksPageViewModel();
    }
}
