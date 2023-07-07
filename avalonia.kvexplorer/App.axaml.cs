using avalonia.kvexplorer.ViewModels;
using avalonia.kvexplorer.Views;

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using FluentAvalonia.UI.Controls;
using kvexplorer.shared;
using Microsoft.Extensions.DependencyInjection;

namespace avalonia.kvexplorer;

public partial class App : Application
{

    public static void ConfigureDefaultServices()
    {
        IServiceCollection serviceCollection = new ServiceCollection();


        // Services
        serviceCollection.AddSingleton<AuthService, AuthService>();
        serviceCollection.AddSingleton<VaultService, VaultService>();

        // ViewModels
        serviceCollection.AddTransient<TitleBarViewModel>();
        serviceCollection.AddTransient<MainViewModel>();

        Defaults.Locator.ConfigureServices(serviceCollection.BuildServiceProvider());
    }
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Line below is needed to remove Avalonia data validation.
        // Without this line you will get duplicate validations from both Avalonia and CT
        BindingPlugins.DataValidators.RemoveAt(0);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainViewModel()
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = new MainViewModel()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
