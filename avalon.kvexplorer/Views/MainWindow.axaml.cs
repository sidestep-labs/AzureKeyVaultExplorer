using Avalonia.Controls;
using Avalonia.Interactivity;
using avalon.kvexplorer.Services;
using Avalonia.Controls.Notifications;
using avalon.kvexplorer.ViewModels;

namespace avalon.kvexplorer.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private MainWindowViewModel _mainWindowViewModel;

    private AuthService _authService;
    public MainWindow(AuthService authService, MainWindowViewModel mainWindowViewModel)
    {
        _authService = authService;
        _mainWindowViewModel = mainWindowViewModel;

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
        var not = new Notification("Test", "this is a test notification message", NotificationType.Warning);
        var nm = new WindowNotificationManager(this)
        {
            Position = NotificationPosition.BottomRight,
            MaxItems = 1
        };
        nm.TemplateApplied += (sender, args) =>
        {
            nm.Show(not);
        };
        var button = (Button)sender;

        button.Content = "Hello, Avalonia!";
    }





}