using Avalonia.Controls;
using Avalonia.Interactivity;
using kvexplorer_av.Services;
using kvexplorer_av.ViewModels;

namespace kvexplorer_av.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }


    public void button_Click(object sender, RoutedEventArgs e)
    {
        // Change button text when button is clicked.

        var button = (Button)sender;

        button.Content = "Hello, Avalonia!";
    }


}