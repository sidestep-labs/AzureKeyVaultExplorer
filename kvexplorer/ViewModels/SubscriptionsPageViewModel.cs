using Avalonia.Threading;
using Azure.Core;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Resources.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using kvexplorer.shared;
using kvexplorer.shared.Database;
using kvexplorer.shared.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace kvexplorer.ViewModels;

public partial class SubscriptionsPageViewModel : ViewModelBase
{
    [ObservableProperty]
    public string continuationToken;

    [ObservableProperty]
    public bool isBusy = true;

    [ObservableProperty]
    public ObservableCollection<SubscriptionDataItems> subscriptions;

    private readonly KvExplorerDb _dbContext;
    private readonly VaultService _vaultService;

    public SubscriptionsPageViewModel()
    {
        _vaultService = Defaults.Locator.GetRequiredService<VaultService>();
        _dbContext = Defaults.Locator.GetRequiredService<KvExplorerDb>();
        Subscriptions = [];
    }

    [RelayCommand]
    public async Task GetAllKeyVaults()
    {
        int count = 0;

        var savedSubscriptions = (await _dbContext.GetAllSubscriptions()).ToDictionary(s => s.SubscriptionId);

        await foreach (var item in _vaultService.GetAllSubscriptions())
        {
            Subscriptions.Add(new SubscriptionDataItems
            {
                Data = item.SubscriptionResource.Data,
                IsPinned = savedSubscriptions.GetValueOrDefault(item.SubscriptionResource.Data.SubscriptionId).SubscriptionId is not null
            });
            count++;
            if (item.ContinuationToken != null && count > 50)
            {
                ContinuationToken = item.ContinuationToken;
                Debug.WriteLine(item.ContinuationToken);
                break;
            }
        }
        IsBusy = false;
    }

    [RelayCommand]
    public void SelectAllSubscriptions()
    {
        foreach (var item in Subscriptions)
            item.IsPinned = true;
        Subscriptions = new ObservableCollection<SubscriptionDataItems>(Subscriptions);
    }

    [RelayCommand]
    public void ClearSelectedSubscriptions()
    {
        foreach (var item in Subscriptions)
            item.IsPinned = false;
        Subscriptions = new ObservableCollection<SubscriptionDataItems>(Subscriptions);
    }

    [RelayCommand]
    public async Task SaveSelectedSubscriptions()
    {
        var selectedItems = Subscriptions.Where(i => i.IsPinned).Select(s => new Subscriptions
        {
            DisplayName = s.Data.DisplayName,
            SubscriptionId = s.Data.SubscriptionId,
            TenantId = s.Data.TenantId ?? Guid.Empty,
        });

        await _dbContext.InsertSubscriptions(selectedItems);
    }
}