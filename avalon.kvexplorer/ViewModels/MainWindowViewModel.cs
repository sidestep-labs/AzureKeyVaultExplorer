using avalon.kvexplorer.Models;
using avalon.kvexplorer.Services;
using Azure.ResourceManager.KeyVault;
using Azure.Security.KeyVault.Secrets;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace avalon.kvexplorer.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{


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

    public MainWindowViewModel(AuthService authService, VaultService vaultService, TitleBarViewModel titleBarViewModel)
    {
        _authService = authService;
        _vaultService = vaultService;
        TitleBarViewModel = titleBarViewModel;

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

    public MainWindowViewModel()
    {
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

    public TitleBarViewModel TitleBarViewModel { get; set; }

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

    //if (SelectedTreeViewItems == null) return;

    //var vault = _vaultService.GetVaultAssociatedSecrets(SelectedTreeViewItems);
    //await foreach (var secret in vault)
    //{
    //    SecretList.Add(secret);
    //}

    private void OnMyViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SelectedTreeItem))
        {
            // Handle changes to the SelectedTreeItem property here
            OnSelectedTreeItemChanged("test");
        }
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
}