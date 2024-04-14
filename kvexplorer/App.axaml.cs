using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using kvexplorer.shared;
using kvexplorer.shared.Database;
using kvexplorer.ViewModels;
using kvexplorer.Views;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace kvexplorer;

public partial class App : Application
{
    public static void ConfigureDesktopServices()
    {
        IServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection.AddMemoryCache();
        serviceCollection.AddSingleton<AuthService, AuthService>();
        serviceCollection.AddSingleton<VaultService, VaultService>();
        serviceCollection.AddSingleton<TabViewPageViewModel>();
        serviceCollection.AddSingleton<ToolBarViewModel>();
        serviceCollection.AddSingleton<KeyVaultTreeListViewModel>();
        serviceCollection.AddTransient<SettingsPageViewModel>();
        serviceCollection.AddSingleton<MainViewModel>();
        serviceCollection.AddSingleton<KvExplorerDb>();
        serviceCollection.AddTransient<AppSettingReader>();
        Defaults.Locator.ConfigureServices(serviceCollection.BuildServiceProvider());
    }

    public static void CreateDesktopResources()
    {
        System.IO.Directory.CreateDirectory(Constants.LocalAppDataFolder);
        var exists = File.Exists(Path.Combine(Constants.LocalAppDataFolder, "kvexplorer.db"));
        if (!exists)
            KvExplorerDb.InitializeDatabase();

        string settingsPath = Path.Combine(Constants.LocalAppDataFolder, "settings.json");
        if (!File.Exists(settingsPath))
            File.WriteAllText(settingsPath, """{ "BackgroundTransparency": false, "NavigationLayoutMode": "Left", "AppTheme": "System" }""");
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
                //DataContext = new MainViewModel()
                DataContext = Defaults.Locator.GetService<MainViewModel>()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}