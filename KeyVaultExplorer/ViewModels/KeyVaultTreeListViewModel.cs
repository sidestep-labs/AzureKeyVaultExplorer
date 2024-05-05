using Avalonia.Threading;
using Azure.Core;
using Azure.ResourceManager;
using Azure.ResourceManager.KeyVault;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KeyVaultExplorer.Database;
using KeyVaultExplorer.Models;
using KeyVaultExplorer.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static KeyVaultExplorer.Services.VaultService;

namespace KeyVaultExplorer.ViewModels;

public partial class KeyVaultTreeListViewModel : ViewModelBase
{
    public IEnumerable<KvSubscriptionModel> _treeViewList;

    [ObservableProperty]
    public bool isBusy = false;

    [ObservableProperty]
    public string searchQuery;

    [ObservableProperty]
    public KeyVaultResource selectedTreeItem;

    [ObservableProperty]
    public ObservableCollection<KvSubscriptionModel> treeViewList;

    private readonly AuthService _authService;
    private readonly KvExplorerDb _dbContext;
    private readonly VaultService _vaultService;
    private readonly string[] WatchedNameOfProps = { nameof(KvSubscriptionModel.IsExpanded), nameof(KvSubscriptionModel.IsSelected) };

    public KeyVaultTreeListViewModel()
    {
        _authService = Defaults.Locator.GetRequiredService<AuthService>();
        _vaultService = Defaults.Locator.GetRequiredService<VaultService>();
        _dbContext = Defaults.Locator.GetRequiredService<KvExplorerDb>();
        // PropertyChanged += OnMyViewModelPropertyChanged;

        TreeViewList = [];
        //foreach (var item in TreeViewList)
        //{
        //    item.PropertyChanged += KvSubscriptionModel_PropertyChanged;
        //}
        // Handle CollectionChanged to attach/detach event handlers for new items
        TreeViewList.CollectionChanged += TreeViewList_CollectionChanged;
        //Dispatcher.UIThread.Post(() => GetAvailableKeyVaults(), DispatcherPriority.Default);
    }

    [RelayCommand]
    public void CollpaseAll()
    {
        foreach (KvSubscriptionModel item in TreeViewList)
        {
            item.IsExpanded = false;
        }
    }

    [RelayCommand]
    public async Task GetAvailableKeyVaults(bool isRefresh = false)
    {
        if (isRefresh)
        {
            TreeViewList.Clear();
        }

        await Dispatcher.UIThread.InvokeAsync(async () =>
        {
            await DelaySetIsBusy(async () =>
            {
                //TODO: get all saved items, otherwise get the first item only.
                var resource = _vaultService.GetKeyVaultResourceBySubscription();

                await foreach (var item in resource)
                {
                    item.PropertyChanged += KvSubscriptionModel_PropertyChanged;
                    item.HasSubNodeDataBeenFetched = false;
                    TreeViewList.Add(item);
                }

                //pinned items, insert the item so it appears instantly, then replace it once it finishes process items from KV
                var quickAccess = new KvSubscriptionModel
                {
                    SubscriptionDisplayName = "Quick Access",
                    SubscriptionId = "",
                    IsExpanded = true,
                    ResourceGroups = [new KvResourceGroupModel { }],
                    Subscription = null,
                };

                TreeViewList.Insert(0, quickAccess);

                var savedItems = _dbContext.GetQuickAccessItemsAsyncEnumerable();
                var token = new CustomTokenCredential(await _authService.GetAzureArmTokenSilent());
                var armClient = new ArmClient(token);
                await foreach (var item in savedItems)
                {
                    var kvr = armClient.GetKeyVaultResource(new ResourceIdentifier(item.KeyVaultId));
                    var kvrResponse = await kvr.GetAsync();
                    //TODO: figure out why i can only have one or the other
                    quickAccess.ResourceGroups[0].KeyVaultResources.Add(kvrResponse);
                    quickAccess.PropertyChanged += KvSubscriptionModel_PropertyChanged;
                }
                quickAccess.ResourceGroups[0].ResourceGroupDisplayName = "Pinned";
                quickAccess.ResourceGroups[0].IsExpanded = true;

                TreeViewList[0] = quickAccess;

                foreach (var sub in TreeViewList)
                {
                    sub.ResourceGroups.CollectionChanged += TreeViewSubNode_CollectionChanged;
                }
            });
        }, DispatcherPriority.Background);
        _treeViewList = TreeViewList;
    }

    // this will set isBusy to true if the fetching takes longer than 1500 ms.
    private async Task DelaySetIsBusy(Func<Task> longRunningTaskFactory)
    {
        IsBusy = false;
        using var cts = new CancellationTokenSource();

        var delayTask = Task.Delay(1500, cts.Token);
        var longRunningTask = longRunningTaskFactory();

        var completedTask = await Task.WhenAny(longRunningTask, delayTask);

        if (completedTask == delayTask)
        {
            // The long-running task took longer than 1500 milliseconds
            IsBusy = true;
            await longRunningTask;
        }
        else
        {
            // The long-running task completed before the delay
            await completedTask;
        }

        IsBusy = false;
    }

    [RelayCommand]
    public async Task PinVaultToQuickAccess(KeyVaultResource model)
    {
        var exists = await _dbContext.QuickAccessItemByKeyVaultIdExists(model.Id);
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

        await _dbContext.InsertQuickAccessItemAsync(qa);

        TreeViewList[0].ResourceGroups[0].KeyVaultResources.Add(model);
        var quickAccess = new KvSubscriptionModel
        {
            SubscriptionDisplayName = "Quick Access",
            IsExpanded = true,
            SubscriptionId = "",
            ResourceGroups = TreeViewList[0].ResourceGroups,
            Subscription = null,
        };
        quickAccess.ResourceGroups[0].ResourceGroupDisplayName = "Pinned";
        quickAccess.ResourceGroups[0].IsExpanded = true;
        TreeViewList[0] = quickAccess;
    }

    [RelayCommand]
    public async Task RemovePinVaultToQuickAccess(KeyVaultResource model)
    {
        var exists = await _dbContext.QuickAccessItemByKeyVaultIdExists(model.Id);
        if (!exists) return;

        await _dbContext.DeleteQuickAccessItemByKeyVaultId(model.Id);

        var rg = TreeViewList[0].ResourceGroups;
        var items = new ObservableCollection<KeyVaultResource>(TreeViewList[0].ResourceGroups[0].KeyVaultResources.Where(s => s.Data.Id != model.Id));
        rg[0].KeyVaultResources = items;
        rg[0].ResourceGroupDisplayName = "Pinned";

        var quickAccess = new KvSubscriptionModel
        {
            SubscriptionDisplayName = "Quick Access",
            IsExpanded = true,
            SubscriptionId = "",
            ResourceGroups = rg,
            Subscription = null,
        };
        TreeViewList[0] = quickAccess;
    }

    private void KvResourceGroupNode_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (WatchedNameOfProps.Contains(e.PropertyName))
        {
            var kvResourceModel = (KvResourceGroupModel)sender;
            // if they are selecting the list item, expand it as a courtesy
            if (e.PropertyName == nameof(KvResourceGroupModel.IsSelected))
                kvResourceModel.IsExpanded = true;
            var hasPlaceholder = kvResourceModel.KeyVaultResources.Any(k => k.GetType().Name == nameof(KeyVaultResourcePlaceholder));
            // if its being expanded and there are no items in the array reach out to azure
            if (kvResourceModel.IsExpanded && hasPlaceholder)
            {
                kvResourceModel.KeyVaultResources.Clear();

                Dispatcher.UIThread.Invoke(async () =>
                {
                    await DelaySetIsBusy(async () =>
                    {
                        var vaults = _vaultService.GetKeyVaultsByResourceGroup(kvResourceModel.ResourceGroupResource);

                        await foreach (var vault in vaults)
                        {
                            kvResourceModel.KeyVaultResources.Add(vault);
                        }
                    });
                }, DispatcherPriority.ContextIdle);
            }
        }
    }

    private void KvSubscriptionModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (WatchedNameOfProps.Contains(e.PropertyName))
        {
            var kvSubModel = (KvSubscriptionModel)sender;
            if (string.IsNullOrWhiteSpace(kvSubModel.SubscriptionId))
                return;

            // if they are selecting the list item, expand it as a courtesy
            if (e.PropertyName == nameof(KvSubscriptionModel.IsSelected))
                kvSubModel.IsExpanded = true;

            bool isExpanded = kvSubModel.IsExpanded;
            var placeholder = new KeyVaultResourcePlaceholder();

            if (isExpanded && !kvSubModel.HasSubNodeDataBeenFetched)
            {
                Dispatcher.UIThread.Invoke(async () =>
                {
                    await DelaySetIsBusy(async () =>
                    {
                        kvSubModel.ResourceGroups.Clear();
                        var resourceGroups = _vaultService.GetResourceGroupBySubscription(kvSubModel);

                        await foreach (var rg in resourceGroups)
                        {
                            kvSubModel.ResourceGroups.Add(
                                new KvResourceGroupModel
                                {
                                    ResourceGroupDisplayName = rg.Data.Name,
                                    ResourceGroupResource = rg,
                                    KeyVaultResources = [placeholder]
                                });
                        }
                        kvSubModel.HasSubNodeDataBeenFetched = true;
                        kvSubModel.ResourceGroups.CollectionChanged += TreeViewSubNode_CollectionChanged;
                    });
                }, DispatcherPriority.ContextIdle);
            }
        }
    }

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
        string query = value.Trim();
        if (!string.IsNullOrWhiteSpace(query))
        {
            TreeViewList = new ObservableCollection<KvSubscriptionModel>(_treeViewList);
        }

        //  var searchValues = SearchValues.Create(query.AsSpan());
        //  var listSearched = _treeViewList.Where(v =>
        //    v.SubscriptionDisplayName.AsSpan().ContainsAny(searchValues) ||
        //    //v.KeyVaultResources.Any(x => x.HasData && x.Data.Name.Contains(query))
        //    v.KeyVaultResources.Any(x => x.HasData && x.Data.Name.AsSpan().ContainsAny(searchValues))
        //);

        var listSearched = _treeViewList.Where(v =>
            v.SubscriptionDisplayName.Contains(query, StringComparison.OrdinalIgnoreCase)
            || v.ResourceGroups.Any(r => r.ResourceGroupDisplayName is not null && r.ResourceGroupDisplayName.Contains(query, StringComparison.OrdinalIgnoreCase)
            || r.KeyVaultResources.Any(kr => kr.HasData && kr.Data.Name.Contains(query, StringComparison.OrdinalIgnoreCase)))
        );
        TreeViewList = new ObservableCollection<KvSubscriptionModel>(listSearched);
    }

    [RelayCommand]
    private void OpenInAzure(KeyVaultResource model)
    {
        if (model is null) return;
        var uri = $"https://portal.azure.com/#@{_authService.TenantName}/resource{model.Id}";
        Process.Start(new ProcessStartInfo(uri) { UseShellExecute = true, Verb = "open" });
    }

    private void TreeViewList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            foreach (KvSubscriptionModel newItem in e.NewItems)
            {
                newItem.PropertyChanged += KvSubscriptionModel_PropertyChanged;
            }
        }
        else if (e.Action == NotifyCollectionChangedAction.Remove)
        {
            foreach (KvSubscriptionModel oldItem in e.OldItems)
            {
                oldItem.PropertyChanged -= KvSubscriptionModel_PropertyChanged;
            }
        }
    }

    private void TreeViewSubNode_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            foreach (KvResourceGroupModel newItem in e.NewItems)
            {
                newItem.PropertyChanged += KvResourceGroupNode_PropertyChanged;
            }
        }
        //else if (e.Action == NotifyCollectionChangedAction.Remove)
        //{
        //    foreach (KvResourceGroupModel oldItem in e.OldItems)
        //    {
        //        oldItem.PropertyChanged -= KvResourceGroupNode_PropertyChanged;
        //    }
        //}
    }
}