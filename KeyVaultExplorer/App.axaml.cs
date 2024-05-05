using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input.Platform;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using Avalonia.VisualTree;
using KeyVaultExplorer.Database;
using KeyVaultExplorer.Models;
using KeyVaultExplorer.Services;
using KeyVaultExplorer.ViewModels;
using KeyVaultExplorer.Views;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace KeyVaultExplorer;

public partial class App : Application
{
    public static void ConfigureDesktopServices()
    {
        IServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection.AddMemoryCache();
        serviceCollection.AddSingleton<AuthService>();
        serviceCollection.AddSingleton<VaultService>();
        serviceCollection.AddSingleton<TabViewPageViewModel>();
        serviceCollection.AddSingleton<ToolBarViewModel>();
        serviceCollection.AddSingleton<KeyVaultTreeListViewModel>();
        serviceCollection.AddSingleton<SettingsPageViewModel>();
        serviceCollection.AddSingleton<MainViewModel>();
        serviceCollection.AddSingleton<NotificationViewModel>();
        serviceCollection.AddSingleton<KvExplorerDb>();
        serviceCollection.AddTransient<AppSettingReader>();
        serviceCollection.AddSingleton<IClipboard, ClipboardService>();
        serviceCollection.AddSingleton<IStorageProvider, StorageProviderService>();
        Defaults.Locator.ConfigureServices(serviceCollection.BuildServiceProvider());
    }

    public static void CreateDesktopResources()
    {
        System.IO.Directory.CreateDirectory(Constants.LocalAppDataFolder);
        var exists = File.Exists(Path.Combine(Constants.LocalAppDataFolder, "KeyVaultExplorer.db"));
        if (!exists)
            KvExplorerDb.InitializeDatabase();

        string settingsPath = Path.Combine(Constants.LocalAppDataFolder, "settings.json");
        if (!File.Exists(settingsPath))
        {
            var s = """
                {
                    "BackgroundTransparency": false,
                    "NavigationLayoutMode": "Left",
                    "AppTheme": "System",
                    "PaneDisplayMode": "inline"
                }
                """;
            File.WriteAllText(settingsPath, s);
        }
    }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Line below is needed to remove Avalonia data validation.
        // Without this line you will get duplicate validations from both Avalonia and CT
        //BindingPlugins.DataValidators.RemoveAt(0);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = Defaults.Locator.GetService<MainViewModel>()
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = Defaults.Locator.GetService<MainViewModel>()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}

public static class ApplicationExtensions
{
    /// <summary>
    /// Returns the TopLevel from the main window or view.
    /// </summary>
    /// <param name="app">The application to get the TopLevel for.</param>
    /// <returns>A TopLevel object.</returns>
    public static TopLevel? GetTopLevel(this Application? app)
    {
        if (app?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            return desktop.MainWindow;
        }
        if (app?.ApplicationLifetime is ISingleViewApplicationLifetime viewApp)
        {
            var visualRoot = viewApp.MainView?.GetVisualRoot();
            return visualRoot as TopLevel;
        }
        return null;
    }
}