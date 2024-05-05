using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using FluentAvalonia.UI.Controls;
using KeyVaultExplorer.Models;
using KeyVaultExplorer.Services;
using KeyVaultExplorer.Views.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace KeyVaultExplorer.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    public string email;

    [ObservableProperty]
    public string initials;

    [ObservableProperty]
    public AuthenticatedUserClaims authenticatedUserClaims;

    [ObservableProperty]
    public bool isAuthenticated = false;

    private readonly AuthService _authService;
    public NavigationFactory NavigationFactory { get; }

    public MainViewModel()
    {
        _authService = Defaults.Locator.GetRequiredService<AuthService>();
        NavigationFactory = new NavigationFactory();
    }

    public async Task RefreshTokenAndGetAccountInformation()
    {
        var cancellation = new CancellationToken();
        var account = await _authService.RefreshTokenAsync(cancellation);

        if (account is null)
            account = await _authService.LoginAsync(cancellation);
        //.ClaimsPrincipal.Identities.First().FindFirst("email").Value.ToLowerInvariant();
        var identity = account.ClaimsPrincipal.Identities.First();
        var email = identity.FindAll("email").First().Value ?? account.Account.Username;

        string[] name = identity.FindAll("name").First().Value.Split(" ");
        if (name.Length > 1)
            Initials = name[0][0].ToString().ToUpperInvariant() + name[1][0].ToString().ToUpperInvariant();

        Email = email.ToLowerInvariant();

        AuthenticatedUserClaims = new AuthenticatedUserClaims()
        {
            Username = account.Account.Username,
            TenantId = account.TenantId,
            Name = account.ClaimsPrincipal.Identities.First().FindFirst("name").Value,
            Email = account.ClaimsPrincipal.Identities.First().FindFirst("email").Value,
        };

        IsAuthenticated = _authService.IsAuthenticated;
    }
}

public class NavigationFactory : INavigationPageFactory
{
    // Do this to avoid needing Activator.CreateInstance to create from type info
    // and to avoid a ridiculous amount of 'ifs'
    private readonly Control[] _pages =
    {
        new MainPage(),
        new SubscriptionsPage(),
        new SettingsPage(),
    };

    private readonly Dictionary<string, Func<Control>> CorePages = new Dictionary<string, Func<Control>>
    {
        { "MainPage", () => new MainPage() },
        { "SubscriptionsPage", () => new SubscriptionsPage() },
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
            SubscriptionsPage => _pages[1],
            SettingsPage => _pages[2],

            _ => throw new Exception()
        };
    }
}