using Avalonia.Controls;
using Avalonia.Interactivity;
using avalon.kvexplorer.Services;

namespace avalon.kvexplorer.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private AuthService _authService;

    public MainWindow(AuthService authService)
    {
        _authService = authService;
    }

    private void OpenWindowButton_Click(object? sender, RoutedEventArgs e)
    {
        // Create the window object
        var sampleWindow =
            new Window
            {
                Title = "Sample Window",
                Width = 200,
                Height = 200
            };

        // open the window
        sampleWindow.Show();
    }

    public void button_Click(object sender, RoutedEventArgs e)
    {
        // Change button text when button is clicked.

        var button = (Button)sender;

        button.Content = "Hello, Avalonia!";
    }
}