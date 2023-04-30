using avalon.kvexplorer.Services;
using avalon.kvexplorer.ViewModels;
using avalon.kvexplorer.Views;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;

namespace avalon.kvexplorer;

public partial class App : Application
{
    public App()
    {
    }

    public static void ConfigureDefaultServices()
    {
        IServiceCollection serviceCollection = new ServiceCollection();

        // Services
        serviceCollection.AddSingleton<AuthService, AuthService>();
        serviceCollection.AddSingleton<VaultService, VaultService>();

        // ViewModels
        serviceCollection.AddTransient<TitleBarViewModel>();
        serviceCollection.AddTransient<MainWindowViewModel>();
        Defaults.Locator.ConfigureServices(serviceCollection.BuildServiceProvider());
    }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        //initialize dependencies
        //var authService = new AuthService();
        //var mainViewModel = new MainWindowViewModel(authService);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Line below is needed to remove Avalonia data validation.
            // Without this line you will get duplicate validations from both Avalonia and CT
            BindingPlugins.DataValidators.RemoveAt(0);
            desktop.MainWindow = new MainWindow
            {
                DataContext = Defaults.Locator.GetService<MainWindowViewModel>()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}