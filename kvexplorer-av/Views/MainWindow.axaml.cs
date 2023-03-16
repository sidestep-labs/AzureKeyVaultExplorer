using Avalonia.Controls;
using Avalonia.Interactivity;
using kvexplorer_av.Services;
using System.Collections.Generic;

namespace kvexplorer_av.Views;

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

    public void button_Click(object sender, RoutedEventArgs e)
    {
        // Change button text when button is clicked.

        var button = (Button)sender;

        button.Content = "Hello, Avalonia!";
    }
}