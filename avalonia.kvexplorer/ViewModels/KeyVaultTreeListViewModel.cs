using Avalonia.Threading;
using Azure.ResourceManager.KeyVault;
using Azure.Security.KeyVault.Secrets;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentAvalonia.UI.Controls;
using kvexplorer.shared;
using kvexplorer.shared.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static kvexplorer.shared.VaultService;

namespace avalonia.kvexplorer.ViewModels;

public partial class KeyVaultTreeListViewModel : ViewModelBase
{
    [ObservableProperty]
    public string searchQuery;

    [ObservableProperty]
    public KeyVaultResource selectedTreeItem;

    [ObservableProperty]
    public ObservableCollection<KeyVaultModel> treeViewList;

    private readonly AuthService _authService;
    private readonly VaultService _vaultService;


    public KeyVaultTreeListViewModel()
    {
        _authService = Defaults.Locator.GetRequiredService<AuthService>();
        _vaultService = Defaults.Locator.GetRequiredService<VaultService>();
        PropertyChanged += OnMyViewModelPropertyChanged;

        treeViewList = new ObservableCollection<KeyVaultModel>
        {
            new KeyVaultModel
            {
                SubscriptionDisplayName = "Sandbox Subscription",
                SubscriptionId = "123",
                KeyVaultResources = new List<KeyVaultResource>{ },
                Subscription = null
            },
        };

       
        //foreach (var item in TreeViewList)
        //{
        //    item.PropertyChanged += KeyVaultModel_PropertyChanged;
        //}
        // Handle CollectionChanged to attach/detach event handlers for new items
        TreeViewList.CollectionChanged += TreeViewList_CollectionChanged;

        //Dispatcher.UIThread.Post(() => GetAvailableKeyVaults(), DispatcherPriority.Default);
    }

    [RelayCommand]
    public async Task GetAvailableKeyVaults()
    {
        await Login();
        var resource = _vaultService.GetKeyVaultResourceBySubscriptionAndResourceGroup();
        await foreach (var item in resource)
        {
            item.PropertyChanged += KeyVaultModel_PropertyChanged;
            TreeViewList.Add(item);
        }
    }


    [RelayCommand]
    private async Task Login()
    {
        var cancellation = new CancellationToken();
        var account = await _authService.RefreshTokenAsync(cancellation);
        if (account == null)
            await _authService.LoginAsync(cancellation);
    }

    private void OnMyViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SelectedTreeItem))
        {
            // Handle changes to the SelectedTreeItem property here
            //OnSelectedTreeItemChanged("test");
        }
    }

    private void TreeViewList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            foreach (KeyVaultModel newItem in e.NewItems)
            {
                newItem.PropertyChanged += KeyVaultModel_PropertyChanged;
            }
        }
        else if (e.Action == NotifyCollectionChangedAction.Remove)
        {
            foreach (KeyVaultModel oldItem in e.OldItems)
            {
                oldItem.PropertyChanged -= KeyVaultModel_PropertyRemoved;
            }
        }
    }

    private void KeyVaultModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(KeyVaultModel.IsExpanded))
        {
            var keyVaultModel = (KeyVaultModel)sender;
            bool isExpanded = keyVaultModel.IsExpanded;
            if (isExpanded && keyVaultModel.KeyVaultResources.Any(k => k.GetType().Name == nameof(KeyVaultResourcePlaceholder)))
            {
                Dispatcher.UIThread.Invoke(() =>
                {
                    _vaultService.UpdateSubscriptionWithKeyVaults(ref keyVaultModel);
                    //keyVaultModel.KeyVaultResources.Clear();
                    //var vaults = _vaultService.GetKeyVaultsBySubscription(keyVaultModel);
                    //foreach (var vault in vaults)
                    //{
                    //    keyVaultModel.KeyVaultResources.Add(vault);
                    //}
                }, DispatcherPriority.ContextIdle);
            }
        }
    }

    private void KeyVaultModel_PropertyRemoved(object sender, PropertyChangedEventArgs e)
    { }

}