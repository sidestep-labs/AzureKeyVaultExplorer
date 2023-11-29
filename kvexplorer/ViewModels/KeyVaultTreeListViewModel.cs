using Avalonia.Animation.Easings;
using Avalonia.Threading;
using Azure.Core;
using Azure.ResourceManager;
using Azure.ResourceManager.KeyVault;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using kvexplorer.shared;
using kvexplorer.shared.Database;
using kvexplorer.shared.Models;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static kvexplorer.shared.VaultService;

namespace kvexplorer.ViewModels;

public partial class KeyVaultTreeListViewModel : ViewModelBase
{
    public IEnumerable<KeyVaultModel> _treeViewList;

    [ObservableProperty]
    public string searchQuery;

    [ObservableProperty]
    public KeyVaultResource selectedTreeItem;

    [ObservableProperty]
    public ObservableCollection<KeyVaultModel> treeViewList;

    [ObservableProperty]
    public bool isBusy;

    private readonly AuthService _authService;
    private readonly VaultService _vaultService;
    private readonly KvExplorerDb _db;

    private readonly string[] WatchedNameOfProps = { nameof(KeyVaultModel.IsExpanded), nameof(KeyVaultModel.IsSelected) };

    public KeyVaultTreeListViewModel()
    {
        _authService = Defaults.Locator.GetRequiredService<AuthService>();
        _vaultService = Defaults.Locator.GetRequiredService<VaultService>();
        _db = Defaults.Locator.GetRequiredService<KvExplorerDb>();
        // PropertyChanged += OnMyViewModelPropertyChanged;

        TreeViewList = new ObservableCollection<KeyVaultModel>
        {
            // new KeyVaultModel
            //{
            //    SubscriptionDisplayName = "Quick Access",
            //    SubscriptionId = "123",
            //    KeyVaultResources = new List<KeyVaultResource>{ },
            //    Subscription = null,
            //    GlyphIcon = "Pin"
            //}, new KeyVaultModel
            //{
            //    SubscriptionDisplayName = "2 Subscription",
            //    SubscriptionId = "123",
            //    KeyVaultResources = new List<KeyVaultResource>{ },
            //    Subscription = null
            //}, new KeyVaultModel
            //{
            //    SubscriptionDisplayName = "3 Subscription",
            //    SubscriptionId = "123",
            //    KeyVaultResources = new List<KeyVaultResource>{ },
            //    Subscription = null
            //},
        };

        //foreach (var item in TreeViewList)
        //{
        //    item.PropertyChanged += KeyVaultModel_PropertyChanged;
        //}
        // Handle CollectionChanged to attach/detach event handlers for new items
        TreeViewList.CollectionChanged += TreeViewList_CollectionChanged;
        //Dispatcher.UIThread.Post(() => GetAvailableKeyVaults(), DispatcherPriority.Default);
    }

    //var kvp = new Azure.ResourceManager.KeyVault.Models.KeyVaultProperties(Guid.Parse(item.TenantId), new KeyVaultSku(KeyVaultSkuFamily.A, KeyVaultSkuName.Standard));
    //kvp.VaultUri = new Uri(item.VaultUri);
    //var kvd = new KeyVaultData(Azure.Core.AzureLocation.EastUS2, kvp)
    //{
    //    Name = item.Name,
    //};
    //var kvr = new KeyVaultResource(armClient, kvd);
    [RelayCommand]
    public async Task GetAvailableKeyVaults(bool isRefresh = false)
    {
        if (isRefresh)
        {
            TreeViewList.Clear();
        }
        await Dispatcher.UIThread.InvokeAsync(async () =>
        {
            //if(!_authService.IsAuthenticated)
            //    return;
            // all items
            var resource = _vaultService.GetKeyVaultResourceBySubscriptionAndResourceGroup();
            await foreach (var item in resource)
            {
                item.PropertyChanged += KeyVaultModel_PropertyChanged;
                item.GlyphIcon = "Permissions";
                TreeViewList.Add(item);
            }

            //pinned items, insert the item so it appears instantly, then replace it once it finishes process items from KV
            var quickAccess = new KeyVaultModel
            {
                SubscriptionDisplayName = "Quick Access",
                SubscriptionId = "",
                IsExpanded = true,
                KeyVaultResources = new List<KeyVaultResource> { },
                Subscription = null,
                GlyphIcon = "ShowResults"
            };
            TreeViewList.Insert(0, quickAccess);

            var savedItems = _db.GetQuickAccessItemsAsyncEnumerable();
            var token = new CustomTokenCredential(await _authService.GetAzureArmTokenSilent());
            var armClient = new ArmClient(token);
            await foreach (var item in savedItems)
            {
                var kvr = armClient.GetKeyVaultResource(new ResourceIdentifier(item.KeyVaultId));
                var kvrResponse = await kvr.GetAsync();
                quickAccess.KeyVaultResources.Add(kvrResponse);
                quickAccess.PropertyChanged += KeyVaultModel_PropertyChanged;
            }
            TreeViewList[0] = quickAccess;
        });

        _treeViewList = TreeViewList;
    }

    [RelayCommand]
    public async Task PinVaultToQuickAccess(KeyVaultResource model)
    {
        var exists = await _db.QuickAccessItemByKeyVaultIdExists(model.Id);
        if (exists) return;
        var qa = new QuickAccess
        {
            KeyVaultId = model.Id,
            Name = model.Data.Name,
            VaultUri = model.Data.Properties.VaultUri.ToString(),
            TenantId = model.Data.Properties.TenantId.ToString(),
            Location = model.Data.Location.Name,
            //SubscriptionDisplayName = model.Data.s
        };

        await _db.InsertQuickAccessItemAsync(qa);

        TreeViewList[0].KeyVaultResources.Add(model);
        var quickAccess = new KeyVaultModel
        {
            SubscriptionDisplayName = "Quick Access",
            IsExpanded = true,
            SubscriptionId = "",
            KeyVaultResources = TreeViewList[0].KeyVaultResources,
            Subscription = null,
            GlyphIcon = "ShowResults"
        };
        TreeViewList[0] = quickAccess;
        //await Dispatcher.UIThread.InvokeAsync(async () =>
        //{
        //    var resource = _vaultService.GetKeyVaultResourceBySubscriptionAndResourceGroup();
        //    await foreach (var item in resource)
        //    {
        //        item.PropertyChanged += KeyVaultModel_PropertyChanged;
        //        TreeViewList.Add(item);
        //    }
        //    _treeViewList = TreeViewList;
        //}, DispatcherPriority.Default);

        //var quickAccess = new KeyVaultModel { SubscriptionDisplayName = "Quick Access", SubscriptionId = "", KeyVaultResources = new List<KeyVaultResource> { }, Subscription = null, GlyphIcon = "Pin" };

        //quickAccess.KeyVaultResources = TreeViewList[0].KeyVaultResources.Where(s => s.Data.Id != item.KeyVaultId).ToList();

        //TreeViewList[0] = quickAccess;

        //_treeViewList = TreeViewList;
    }

    [RelayCommand]
    public async Task RemovePinVaultToQuickAccess(KeyVaultResource model)
    {
        var exists = await _db.QuickAccessItemByKeyVaultIdExists(model.Id);
        if (!exists) return;

        await _db.DeleteQuickAccessItemByKeyVaultId(model.Id);

        var quickAccess = new KeyVaultModel
        {
            SubscriptionDisplayName = "Quick Access",
            IsExpanded = true,
            SubscriptionId = "",
            KeyVaultResources = TreeViewList[0].KeyVaultResources.Where(s => s.Data.Id != model.Id).ToList(),
            Subscription = null,
            GlyphIcon = "Pin"
        };
        TreeViewList[0] = quickAccess;
    }

    private void KeyVaultModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (WatchedNameOfProps.Contains(e.PropertyName))
        {
            var keyVaultModel = (KeyVaultModel)sender;
            // if they are selecting the list item, expand it as a courtesy
            if (e.PropertyName == nameof(KeyVaultModel.IsSelected))
                keyVaultModel.IsExpanded = true;

            bool isExpanded = keyVaultModel.IsExpanded;
            if (isExpanded && keyVaultModel.KeyVaultResources.Any(k => k.GetType().Name == nameof(KeyVaultResourcePlaceholder)))
            {
                Dispatcher.UIThread.Invoke(async () =>
                {
                    //_vaultService.UpdateSubscriptionWithKeyVaults(ref keyVaultModel); /* This does not work with AOT */
                    keyVaultModel.KeyVaultResources.Clear();
                    var vaults = _vaultService.GetKeyVaultsBySubscription(keyVaultModel);
                    await foreach (var vault in vaults)
                    {
                        keyVaultModel.KeyVaultResources.Add(vault);
                    }
                }, DispatcherPriority.ContextIdle);
            }
        }
    }

    public void KeyVaultModel_PropertyRemoved(object sender, PropertyChangedEventArgs e)
    { }

   

    //private void OnMyViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    //{
    //    if (e.PropertyName == nameof(SelectedTreeItem))
    //    {
    //        // Handle changes to the SelectedTreeItem property here
    //        //OnSelectedTreeItemChanged("test");
    //    }
    //}

    partial void OnSearchQueryChanged(string value)
    {
        string query = value.Trim().ToLowerInvariant();
        if (!string.IsNullOrWhiteSpace(query))
        {
            TreeViewList = new ObservableCollection<KeyVaultModel>(_treeViewList);
        }
        var list = _treeViewList.Where(v => v.SubscriptionDisplayName.ToLowerInvariant().Contains(query));
        TreeViewList = new ObservableCollection<KeyVaultModel>(list);
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



    [RelayCommand]
    private void OpenInAzure(KeyVaultResource model)
    {
        if (model is null) return;
        var tenantName = _authService.Account.Username.Split("@").TakeLast(1).Single();
        var uri = $"https://portal.azure.com/#@{tenantName}/resource{model.Id}";
        Process.Start(new ProcessStartInfo(uri) { UseShellExecute = true, Verb = "open" });
    }

}