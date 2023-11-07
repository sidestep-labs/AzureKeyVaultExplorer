using avalonia.kvexplorer.ViewModels;
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

namespace avalonia.kvexplorer.Views;

public partial class MainWindow : AppWindow
{

    public MainWindow()
    {
        InitializeComponent();
        TitleBar.ExtendsContentIntoTitleBar = true;
        TitleBar.TitleBarHitTestType = TitleBarHitTestType.Complex;
        //TitleBar.ButtonHoverBackgroundColor = Color.FromArgb(35, 155, 155, 155);

        Activated += (sender, e) =>
        {
            Background = null;
        };
        Deactivated += (sender, e) =>
        {
            App.Current.Resources.TryGetResource("DynamicActiveBackgroundFAColor", null, out var bg);
            Background = (IBrush)bg;
        };


        //TitleBar.ExtendsContentIntoTitleBar = OperatingSystem.IsMacOS() ? true : false;
        // ExtendClientAreaChromeHints = OperatingSystem.IsMacOS() ? Avalonia.Platform.ExtendClientAreaChromeHints.NoChrome : Avalonia.Platform.ExtendClientAreaChromeHints.Default;
    }

}