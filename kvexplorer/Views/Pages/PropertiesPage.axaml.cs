using kvexplorer.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Controls.Notifications;
using Azure.Security.KeyVault.Keys;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using System;
using kvexplorer.shared.Models;
using kvexplorer.shared;
using Avalonia.Input;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input.Platform;

namespace kvexplorer;

public partial class PropertiesPage : UserControl
{
    private VaultService _vaultService;


    public PropertiesPage()
    {
        InitializeComponent();
        DataContext = new PropertiesPageViewModel();
        _vaultService = Defaults.Locator.GetRequiredService<VaultService>();
    }

 


}