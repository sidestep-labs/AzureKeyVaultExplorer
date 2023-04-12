using avalon.kvexplorer.Models;
using avalon.kvexplorer.Services;
using Azure.ResourceManager.KeyVault;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace avalon.kvexplorer.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly AuthService _authService;
    private readonly VaultService _vaultService;

    [ObservableProperty]
    public List<MyData> listOfPeople;

    [ObservableProperty]
    public List<KeyVaultResource> vaultList;

    [ObservableProperty]
    public ObservableCollection<KeyVaultModel> vaultTreeList;

    public MainWindowViewModel(AuthService authService, VaultService vaultService)
    {
        _authService = authService;
        _vaultService = vaultService;
        vaultList = new List<KeyVaultResource>();
        //vaultTreeList = new ObservableCollection<KeyVaultModel>();

        vaultTreeList = new ObservableCollection<KeyVaultModel>
        {
            new KeyVaultModel {
                SubscriptionDisplayName = "Sandbox Subscription",
                SubscriptionId = "123",
                KeyVaultResources = new List<KeyVaultResource>{ } },

            new KeyVaultModel { SubscriptionDisplayName = "Development", SubscriptionId = "123" },
            new KeyVaultModel { SubscriptionDisplayName = "QA", SubscriptionId = "123" },
        };

        listOfPeople = new List<MyData>
        {
            new MyData { Name = "John Doe", Age = 42, Address = "123 Main St." },
            new MyData { Name = "Jane Doe", Age = 39, Address = "456 Oak St." },
            new MyData { Name = "Bob Smith", Age = 27, Address = "789 Elm St." }
        };

        Task.Run(async () =>
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
        };

        listOfPeople = new List<MyData>
        {
            new MyData { Name = "John Doe", Age = 42, Address = "123 Main St." },
            new MyData { Name = "Jane Doe", Age = 39, Address = "456 Oak St." },
            new MyData { Name = "Bob Smith", Age = 27, Address = "789 Elm St." }
        };
    }

    [RelayCommand]
    private async void Login()
    {
        var cancellation = new CancellationToken();
        var account = await _authService.RefreshTokenAsync(cancellation);
        if (account == null)
            await _authService.LoginAsync(cancellation);
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

    public string Greeting => "Welcome to Avalonia!";
}