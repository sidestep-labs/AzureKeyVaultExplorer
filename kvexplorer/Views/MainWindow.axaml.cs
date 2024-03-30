using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using FluentAvalonia.UI.Windowing;
using kvexplorer.shared;
using kvexplorer.shared.Database;
using kvexplorer.shared.Models;
using kvexplorer.ViewModels;
using kvexplorer.Views.Pages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;

namespace kvexplorer.Views;

public partial class MainWindow : AppWindow
{
    public static readonly RoutedEvent<RoutedEventArgs> TransparencyChangedEvent = RoutedEvent.Register<MainView, RoutedEventArgs>(nameof(TransparencyChangedEvent), RoutingStrategies.Tunnel);
    private IBrush BackgroundBrush;

    private bool TransparencyEnabled { get; set; }

    public MainWindow()
    {
        InitializeComponent();
        TitleBar.ExtendsContentIntoTitleBar = true;
        TitleBar.TitleBarHitTestType = TitleBarHitTestType.Complex;
        AddHandler(TransparencyChangedEvent, OnTransparencyChangedEvent, RoutingStrategies.Tunnel, handledEventsToo: false);

        //TitleBar.ButtonHoverBackgroundColor = Color.FromArgb(35, 155, 155, 155);
        App.Current.Resources.TryGetResource("DynamicActiveBackgroundFAColor", null, out var bg);
        BackgroundBrush = (IBrush)bg;
        var path = Path.Combine(Constants.LocalAppDataFolder, "settings.json");
        var settings = Defaults.Locator.GetRequiredService<AppSettingReader>();
        TransparencyEnabled = settings.AppSettings.BackgroundTransparency;

        // TODO: get background transparency from db to know if to enable this on startup.
        if (TransparencyEnabled)
        {
            Background = null;
        }

        if (OperatingSystem.IsWindows())
        {
            Activated += (sender, e) =>
            {
                if (TransparencyEnabled)
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

    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);

        if (TitleBar != null)
        {
            TitleBar.ExtendsContentIntoTitleBar = true;
            TitleBar.TitleBarHitTestType = TitleBarHitTestType.Complex;

            var parent = this.FindControl<Views.MainView>("MainView")!
                .FindControl<FluentAvalonia.UI.Controls.Frame>("FrameView");

            var tab = ((MainPage)parent.Content).Content as TabViewPage;
            var dragRegion = tab.FindControl<Panel>("CustomDragRegion");

            dragRegion.MinWidth = FlowDirection == Avalonia.Media.FlowDirection.LeftToRight ?
                TitleBar.RightInset : TitleBar.LeftInset;
        }
    }
    private void OnTransparencyChangedEvent(object sender, RoutedEventArgs e)
    {
        var isChecked = (e.Source as CheckBox)?.IsChecked ?? false;
        if (OperatingSystem.IsWindows() && isChecked)
        {
            Background = null;
            TransparencyEnabled = true;
        }
        else
        {
            Background = BackgroundBrush;
            TransparencyEnabled = false;
        }
        e.Handled = true;
    }
}