using avalonia.kvexplorer.ViewModels;
using avalonia.kvexplorer.Views;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using kvexplorer.shared;
using Microsoft.Extensions.DependencyInjection;

namespace avalonia.kvexplorer;

public partial class App : Application
{
    public static void ConfigureDesktopServices()
    {
        IServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<AuthService, AuthService>();
        serviceCollection.AddSingleton<VaultService, VaultService>();
        serviceCollection.AddTransient<SettingsPageViewModel>();
        serviceCollection.AddTransient<MainViewModel>();
        serviceCollection.AddSingleton<TabViewPageViewModel>();

        Defaults.Locator.ConfigureServices(serviceCollection.BuildServiceProvider());
    }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        Defaults.Locator.GetService<AuthService>();

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