using avalon.kvexplorer.Models;
using avalon.kvexplorer.Services;
using Avalonia.Controls;
using Azure.ResourceManager.KeyVault;
using Azure.Security.KeyVault.Secrets;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace avalon.kvexplorer.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly AuthService _authService;
    private readonly VaultService _vaultService;
    public TitleBarViewModel TitleBarViewModel { get; set; }

    [ObservableProperty]
    public List<MyData> listOfPeople;


    [ObservableProperty]
    public string searchQuery;
    //[ObservableProperty]
    //public List<KeyVaultResource> vaultList;

    [ObservableProperty]
    public ObservableCollection<KeyVaultModel> vaultTreeList;

    [ObservableProperty]
    public KeyVaultResource selectedTreeItem;

    [ObservableProperty]
    private ObservableCollection<SecretProperties> secretList;

    public class Person
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int Age { get; set; }
    }

    private ObservableCollection<Person> _people = new()
    {
        new Person { FirstName = "Eleanor", LastName = "Pope", Age = 32 },
        new Person { FirstName = "Jeremy", LastName = "Navarro", Age = 74 },
        new Person { FirstName = "Lailah ", LastName = "Velazquez", Age = 16 },
        new Person { FirstName = "Jazmine", LastName = "Schroeder", Age = 52 },
    };

    public MainWindowViewModel(AuthService authService, VaultService vaultService, TitleBarViewModel titleBarViewModel)
    {
        _authService = authService;
        _vaultService = vaultService;
        TitleBarViewModel = titleBarViewModel;

        SecretList = new ObservableCollection<SecretProperties>();
        PropertyChanged += OnMyViewModelPropertyChanged;

        //vaultList = new List<KeyVaultResource>();
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
            new KeyVaultModel { SubscriptionDisplayName = "Production", SubscriptionId = "5" },
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


        //if (SelectedTreeViewItems == null) return;

        //var vault = _vaultService.GetVaultAssociatedSecrets(SelectedTreeViewItems);
        //await foreach (var secret in vault)
        //{
        //    SecretList.Add(secret);
        //}



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