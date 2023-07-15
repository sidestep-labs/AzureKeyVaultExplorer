using avalonia.kvexplorer.Views.Pages;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Azure.ResourceManager.KeyVault;
using Azure.Security.KeyVault.Secrets;
using CommunityToolkit.Mvvm.ComponentModel;
using FluentAvalonia.UI.Controls;
using kvexplorer.shared;
using kvexplorer.shared.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Threading;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace avalonia.kvexplorer.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly AuthService _authService;
    public NavigationFactory NavigationFactory { get; }

    [ObservableProperty]
    public string email = "unathenticated";

    public MainViewModel()
    {
        _authService = Defaults.Locator.GetRequiredService<AuthService>();
        NavigationFactory = new NavigationFactory();

        Dispatcher.UIThread.InvokeAsync(() => _ = RefreshTokenAndGetAccountInformation());
    }

    public async Task RefreshTokenAndGetAccountInformation()
    {
        var cancellation = new CancellationToken();
        var account = await _authService.RefreshTokenAsync(cancellation);

        if (account is null)
            account = await _authService.LoginAsync(cancellation);

        Email = account.ClaimsPrincipal.Identities.FirstOrDefault().FindFirst("email").Value ?? account.Account.Username;
    }
}

public class NavigationFactory : INavigationPageFactory
{
    public NavigationFactory()
    {
        Instance = this;
    }

    private static NavigationFactory? Instance { get; set; }

    // Create a page based on a Type, but you can create it however you want
    public Control? GetPage(Type srcType)
    {
        // Return null here because we won't use this method at all
        CorePages.TryGetValue(srcType.FullName, out var func);
        Control page = null;
        page = func();
        return page;
    }

    // Create a page based on an object, such as a view model
    public Control? GetPageFromObject(object target)
    {
        return target switch
        {
            MainPage => _pages[0],
            BookmarksPage => _pages[1],
            SettingsPage => _pages[2],

            _ => throw new Exception()
        };
    }

    // Do this to avoid needing Activator.CreateInstance to create from type info
    // and to avoid a ridiculous amount of 'ifs'
    private readonly Control[] _pages =
    {
        new MainPage(),
        new BookmarksPage(),
        new SettingsPage(),
    };

    private readonly Dictionary<string, Func<Control>> CorePages = new Dictionary<string, Func<Control>>
    {
        { "MainPage", () => new MainPage() },
        { "BookmarksPage", () => new BookmarksPage() },
        { "SettingsPage", () => new SettingsPage() },
    };

    public static Control[] GetPages()
    {
        return Instance!._pages;
    }
}