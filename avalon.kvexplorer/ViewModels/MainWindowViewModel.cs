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
        vaultTreeList = new ObservableCollection<KeyVaultModel>();

        //vaultTreeList = new List<KeyVaultModel>
        //{
        //    new KeyVaultModel { SubscriptionDisplayName = "test1", SubscriptionId = "123" },
        //    new KeyVaultModel { SubscriptionDisplayName = "test2", SubscriptionId = "123" },

        //    };

        listOfPeople = new List<MyData>
        {
            new MyData { Name = "John Doe", Age = 42, Address = "123 Main St." },
            new MyData { Name = "Jane Doe", Age = 39, Address = "456 Oak St." },
            new MyData { Name = "Bob Smith", Age = 27, Address = "789 Elm St." }
        };

        Task.Run(async () =>
        {
            GetAvailableKeyVaultsxx();
        });
    }

    public MainWindowViewModel() { }


    [RelayCommand]
    private async void Login()
    {
        var cancellation = new CancellationToken();
        var account = await _authService.RefreshTokenAsync(cancellation);
        if (account == null)
        {
            await _authService.LoginAsync(cancellation);
        }
        var resource = _vaultService.GetKeyVaultResourceBySubscriptionAndResourceGroup();
        await foreach (var item in resource)
        {
            vaultTreeList.Add(item);
        }
    }

    [RelayCommand]
    private async void GetAvailableKeyVaults()
    {
        var cancellation = new CancellationToken();
        var account = await _authService.RefreshTokenAsync(cancellation);
        if (account == null)
            await _authService.LoginAsync(cancellation);
        var resource = _vaultService.GetKeyVaultResourceBySubscriptionAndResourceGroup();
        await foreach (var item in resource)
        {
            vaultTreeList.Add(item);
        }
    }

    private async void GetAvailableKeyVaultsxx()
    {
        var cancellation = new CancellationToken();
        var account = await _authService.RefreshTokenAsync(cancellation);
        if (account == null)
            await _authService.LoginAsync(cancellation);
        var resource = _vaultService.GetKeyVaultResourceBySubscriptionAndResourceGroup();
        await foreach (var item in resource)
        {
            vaultTreeList.Add(item);
        }
    }

    public string Greeting => "Welcome to Avalonia!";
}