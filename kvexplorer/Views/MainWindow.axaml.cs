using kvexplorer.ViewModels;
using kvexplorer.Views.Pages;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Styling;
using FluentAvalonia.Styling;
using FluentAvalonia.UI.Windowing;
using System;
using System.Diagnostics;
using System.Reflection;
using kvexplorer.shared.Database;
using System.Configuration;

namespace kvexplorer.Views;

public partial class MainWindow : AppWindow
{
    public static readonly RoutedEvent<RoutedEventArgs> TransparencyChangedEvent = RoutedEvent.Register<MainView, RoutedEventArgs>(nameof(TransparencyChangedEvent), RoutingStrategies.Tunnel);
    private IBrush BackgroundBrush;
    private readonly KvExplorerDb _db;

    private SettingsPageViewModel _settingsPageViewModel;

    public MainWindow()
    {
        InitializeComponent();
        TitleBar.ExtendsContentIntoTitleBar = true;
        TitleBar.TitleBarHitTestType = TitleBarHitTestType.Complex;
        AddHandler(TransparencyChangedEvent, OnTransparencyChangedEvent, RoutingStrategies.Tunnel, handledEventsToo: false);

        //TitleBar.ButtonHoverBackgroundColor = Color.FromArgb(35, 155, 155, 155);
        App.Current.Resources.TryGetResource("DynamicActiveBackgroundFAColor", null, out var bg);
        BackgroundBrush = (IBrush)bg;
        
        //var  configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        var isMicaEnabled = false;
        // TODO: get background trancy from db to know if to enable this on startup.
        if (isMicaEnabled)
            Background = null;

        if (OperatingSystem.IsWindows())
        {
            Activated += (sender, e) =>
            {
                // TODO: get background trancy from db to know if to enable this on startup.
                if (isMicaEnabled)
                    Background = null;
            };
            Deactivated += (sender, e) =>
            {
                Background = BackgroundBrush;
            };
        }
        else
        {
      
                Background = BackgroundBrush;
        }

        //TitleBar.ExtendsContentIntoTitleBar = OperatingSystem.IsMacOS() ? true : false;
        // ExtendClientAreaChromeHints = OperatingSystem.IsMacOS() ? Avalonia.Platform.ExtendClientAreaChromeHints.NoChrome : Avalonia.Platform.ExtendClientAreaChromeHints.Default;
    }

    private void OnTransparencyChangedEvent(object sender, RoutedEventArgs e)
    {
        var isChecked = (e.Source as CheckBox)?.IsChecked ?? false;
        if (OperatingSystem.IsWindows() && isChecked)
        {
            Background = null;
        }
        else
        {
            Background = BackgroundBrush;
        }
        e.Handled = true;
    }
}