using avalonia.kvexplorer.Views.Pages;
using Avalonia.Controls;
using Azure.ResourceManager.KeyVault;
using Azure.Security.KeyVault.Secrets;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentAvalonia.UI.Controls;
using kvexplorer.shared;
using kvexplorer.shared.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace avalonia.kvexplorer.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public string Greeting => "Welcome to Avalonia!";
    public TitleBarViewModel TitleBarViewModel { get; set; }

    [ObservableProperty]
    public string searchQuery;

    [ObservableProperty]
    public KeyVaultResource selectedTreeItem;

    [ObservableProperty]
    public ObservableCollection<KeyVaultModel> vaultTreeList;

    private readonly AuthService _authService;
    private readonly VaultService _vaultService;

    [ObservableProperty]
    private ObservableCollection<SecretProperties> secretList;

    public NavigationFactory NavigationFactory { get; }

    public MainViewModel(AuthService authService, VaultService vaultService, TitleBarViewModel titleBarViewModel)
    {
        NavigationFactory = new NavigationFactory();
        _authService = authService;
        _vaultService = vaultService;
        TitleBarViewModel = titleBarViewModel;
        //TestFrame.Navigate(typeof(SamplePage));
        //SecretList = new ObservableCollection<SecretProperties>();
        PropertyChanged += OnMyViewModelPropertyChanged;

        //vaultList = new List<KeyVaultResource>();
        vaultTreeList = new ObservableCollection<KeyVaultModel>
        {
            new KeyVaultModel
            {
                SubscriptionDisplayName = "Sandbox Subscription",
                SubscriptionId = "123",
                KeyVaultResources = new List<KeyVaultResource>{ }
            },

            new KeyVaultModel { SubscriptionDisplayName = "Sandbox Subscription", SubscriptionId = "1" },
            new KeyVaultModel { SubscriptionDisplayName = "Development", SubscriptionId = "2" },
            new KeyVaultModel { SubscriptionDisplayName = "QA", SubscriptionId = "3" },
            new KeyVaultModel { SubscriptionDisplayName = "Production", SubscriptionId = "5" },
            new KeyVaultModel { SubscriptionDisplayName = "Sandbox Subscription", SubscriptionId = "1" },
            new KeyVaultModel { SubscriptionDisplayName = "Development", SubscriptionId = "2" },
            new KeyVaultModel { SubscriptionDisplayName = "QA", SubscriptionId = "3" },
            new KeyVaultModel { SubscriptionDisplayName = "Production", SubscriptionId = "5" },
            new KeyVaultModel { SubscriptionDisplayName = "Sandbox Subscription", SubscriptionId = "1"  },
            new KeyVaultModel { SubscriptionDisplayName = "Development", SubscriptionId = "2" },
            new KeyVaultModel { SubscriptionDisplayName = "QA", SubscriptionId = "3" },
            new KeyVaultModel { SubscriptionDisplayName = "Production", SubscriptionId = "5" },
        };

        secretList = new()
            {
                new SecretProperties("Salesforce Password") { ContentType = "application/json", Enabled = true, ExpiresOn = new System.DateTime(), },
                new SecretProperties("AMEX Card") { ContentType = "application/json", Enabled = true, ExpiresOn = new System.DateTime(), },
                new SecretProperties("shared dev key") { ContentType = "text", Enabled = true, ExpiresOn = new System.DateTime(), },
                new SecretProperties("SnowflakeIntegrationUsername") { ContentType = "text", Enabled = true, ExpiresOn = new System.DateTime(), },
                new SecretProperties("AzClientID") { ContentType = "guid", Enabled = true, ExpiresOn = new System.DateTime(), },
                new SecretProperties("SnowflakeIntegrationpassword") { ContentType = "text", Enabled = true, ExpiresOn = new System.DateTime(), },
                new SecretProperties("YoutubeAPIKey") { ContentType = "guid", Enabled = true, ExpiresOn = new System.DateTime(), },
            };

        Task.Run(() =>
        {
            GetAvailableKeyVaults();
        });
    }

    public MainViewModel()
    {
        NavigationFactory = new NavigationFactory();
        vaultTreeList = new ObservableCollection<KeyVaultModel>
        {
            new KeyVaultModel { SubscriptionDisplayName = "Sandbox Subscription", SubscriptionId = "1" },
            new KeyVaultModel { SubscriptionDisplayName = "Development", SubscriptionId = "2" },
            new KeyVaultModel { SubscriptionDisplayName = "QA", SubscriptionId = "3" },
            new KeyVaultModel { SubscriptionDisplayName = "Production", SubscriptionId = "5" },
            new KeyVaultModel { SubscriptionDisplayName = "Sandbox Subscription", SubscriptionId = "1" },
            new KeyVaultModel { SubscriptionDisplayName = "Development", SubscriptionId = "2" },
            new KeyVaultModel { SubscriptionDisplayName = "QA", SubscriptionId = "3" },
            new KeyVaultModel { SubscriptionDisplayName = "Production", SubscriptionId = "5" },
            new KeyVaultModel { SubscriptionDisplayName = "Sandbox Subscription", SubscriptionId = "1"  },
            new KeyVaultModel { SubscriptionDisplayName = "Development", SubscriptionId = "2" },
            new KeyVaultModel { SubscriptionDisplayName = "QA", SubscriptionId = "3" },
            new KeyVaultModel { SubscriptionDisplayName = "Production", SubscriptionId = "5" },
        };

        secretList = new()
            {
                new SecretProperties("Salesforce Password") { ContentType = "application/json", Enabled = true, ExpiresOn = new System.DateTime(), },
                new SecretProperties("AMEX Card") { ContentType = "application/json", Enabled = true, ExpiresOn = new System.DateTime(), },
                new SecretProperties("shared dev key") { ContentType = "text", Enabled = true, ExpiresOn = new System.DateTime(), },
                new SecretProperties("SnowflakeIntegrationUsername") { ContentType = "text", Enabled = true, ExpiresOn = new System.DateTime(), },
                new SecretProperties("AzClientID") { ContentType = "guid", Enabled = true, ExpiresOn = new System.DateTime(), },
                new SecretProperties("SnowflakeIntegrationpassword") { ContentType = "text", Enabled = true, ExpiresOn = new System.DateTime(), },
                new SecretProperties("YoutubeAPIKey") { ContentType = "guid", Enabled = true, ExpiresOn = new System.DateTime(), },
                new SecretProperties("shared dev key") { ContentType = "text", Enabled = true, ExpiresOn = new System.DateTime(), },
                new SecretProperties("SnowflakeIntegrationUsername") { ContentType = "text", Enabled = true, ExpiresOn = new System.DateTime(), },
                new SecretProperties("AzClientID") { ContentType = "guid", Enabled = true, ExpiresOn = new System.DateTime(), },
                new SecretProperties("SnowflakeIntegrationUsername") { ContentType = "text", Enabled = true, ExpiresOn = new System.DateTime(), },
                new SecretProperties("AzClientID") { ContentType = "guid", Enabled = true, ExpiresOn = new System.DateTime(), },
                new SecretProperties("SnowflakeIntegrationpassword") { ContentType = "text", Enabled = true, ExpiresOn = new System.DateTime(), },
                new SecretProperties("YoutubeAPIKey") { ContentType = "guid", Enabled = true, ExpiresOn = new System.DateTime(), },
            };
    }

    [RelayCommand]
    private async void GetAvailableKeyVaults()
    {
        Login();
        var resource = _vaultService.GetKeyVaultResourceBySubscriptionAndResourceGroup();
        await foreach (var item in resource)
        {
            VaultTreeList.Add(item);
        }
    }

    [RelayCommand]
    private async void Login()
    {
        var cancellation = new CancellationToken();
        var account = await _authService.RefreshTokenAsync(cancellation);
        if (account == null)
            await _authService.LoginAsync(cancellation);
    }


 
    private async void OnSelectedTreeItemChanged(object value)
    {
        // Handle the SelectedTreeItem property change event here
        if (SelectedTreeItem == null) return;
        var vault = _vaultService.GetVaultAssociatedSecrets(SelectedTreeItem.Data.Properties.VaultUri);
        await foreach (var secret in vault)
        {
            SecretList.Add(secret);
            Debug.WriteLine($"value, {value}");
        }
    }

    private void OnMyViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SelectedTreeItem))
        {
            // Handle changes to the SelectedTreeItem property here
            OnSelectedTreeItemChanged("test");
        }
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
            WelcomePage => _pages[1],
            SettingsPage => _pages[2],

            _ => throw new Exception()
        };
    }

    // Do this to avoid needing Activator.CreateInstance to create from type info
    // and to avoid a ridiculous amount of 'ifs'
    private readonly Control[] _pages =
    {
        new MainPage(), 
        new WelcomePage(),
        new SettingsPage(),

    };

   

    private readonly Dictionary<string, Func<Control>> CorePages = new Dictionary<string, Func<Control>>
    {
        { "MainPage", () => new MainPage() },
        { "WelcomePage", () => new WelcomePage() },
        { "SettingsPage", () => new SettingsPage() },
    };
    public static Control[] GetPages()
    {
        return Instance!._pages;
    }
}