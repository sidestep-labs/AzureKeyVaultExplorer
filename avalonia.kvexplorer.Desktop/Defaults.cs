using CommunityToolkit.Mvvm.DependencyInjection;

namespace avalonia.kvexplorer;

public static class Defaults
{
    public const string WelcomeMessage = "Azure KeyVault Explorer";
    public static Ioc Locator = Ioc.Default;
}