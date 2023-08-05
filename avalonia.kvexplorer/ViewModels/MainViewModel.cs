using avalonia.kvexplorer.Views.Pages;
using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using FluentAvalonia.UI.Controls;
using kvexplorer.shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace avalonia.kvexplorer.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    public string email = "unauthenticated";

    private readonly AuthService _authService;
    public NavigationFactory NavigationFactory { get; }

    public MainViewModel()
    {
        _authService = Defaults.Locator.GetRequiredService<AuthService>();
        NavigationFactory = new NavigationFactory();

        Dispatcher.UIThread.Post(() => _ = RefreshTokenAndGetAccountInformation());
    }

    public async Task RefreshTokenAndGetAccountInformation()
    {
        var cancellation = new CancellationToken();
        var account = await _authService.RefreshTokenAsync(cancellation);

        if (account is null)
            account = await _authService.LoginAsync(cancellation);
        //.ClaimsPrincipal.Identities.First().FindFirst("email").Value.ToLowerInvariant();
        var email = account.ClaimsPrincipal.Identities.First().FindAll("email").First().Value ?? account.Account.Username;

        Email = email.ToLowerInvariant();
    }
}

public class NavigationFactory : INavigationPageFactory
{
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

    public NavigationFactory()
    {
        Instance = this;
    }

    private static NavigationFactory? Instance { get; set; }

    public static Control[] GetPages()
    {
        return Instance!._pages;
    }

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
}