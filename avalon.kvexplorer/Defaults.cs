using avalon.kvexplorer.ViewModels;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace avalon.kvexplorer;

public static class Defaults
{
    public const string WelcomeMessage = "Azure KeyVault Explorer";

    public static Ioc Locator = Ioc.Default;

    //public static IServiceProvider ConfigureDefaultServices()
    //{
    //    IServiceCollection serviceCollection = new ServiceCollection();

    //    serviceCollection.AddSingleton<AuthService, AuthService>();
    //    serviceCollection.AddSingleton<VaultService, VaultService>();

    //    serviceCollection.AddTransient<MainWindowViewModel>();

    //    IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

    //    Locator.ConfigureServices(serviceProvider);

    //    return serviceProvider;
    //}
}