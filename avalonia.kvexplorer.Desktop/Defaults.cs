using avalonia.kvexplorer.ViewModels;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace avalonia.kvexplorer;

public static class Defaults
{
    public const string WelcomeMessage = "Azure KeyVault Explorer";
    public static Ioc Locator = Ioc.Default;
}