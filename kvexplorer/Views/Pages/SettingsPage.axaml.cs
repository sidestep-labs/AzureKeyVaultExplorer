using kvexplorer.ViewModels;
using Avalonia.Controls;
using Avalonia.Threading;
using FluentAvalonia.UI.Controls;
using Avalonia.Interactivity;
using Avalonia;
using System;

namespace kvexplorer.Views.Pages;

public partial class SettingsPage : UserControl
{
    public SettingsPage()
    {
        InitializeComponent();
        //DataContext = new SettingsPageViewModel();
        DataContext = Defaults.Locator.GetRequiredService<SettingsPageViewModel>();
        var bgCheckbox = this.FindControl<CheckBox>("BackgroundTransparencyCheckbox");
        bgCheckbox.IsCheckedChanged += BackgroundTransparency_ChangedEvent;
    }

    private void BackgroundTransparency_ChangedEvent(object? sender, RoutedEventArgs e)
    {
        Control control = (Control)sender!;
        control.RaiseEvent(new RoutedEventArgs(MainWindow.TransparencyChangedEvent));
    }

    private void FetchUserInfoSettingsExpanderItem_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        (DataContext as SettingsPageViewModel).SignInOrRefreshTokenCommand.Execute(null);
    }

}