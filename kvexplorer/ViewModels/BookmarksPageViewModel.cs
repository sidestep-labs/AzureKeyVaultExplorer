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
using System.Threading.Tasks;

namespace kvexplorer.ViewModels;

public partial class BookmarksPageViewModel : ViewModelBase
{
    [ObservableProperty]
    public string continuationToken = "";

    [ObservableProperty]
    public bool isBusy = true;

    [ObservableProperty]
    public ObservableCollection<SubscriptionDataItems> subscriptions;

    private readonly KvExplorerDb _db;
    private readonly VaultService _vaultService;

    public BookmarksPageViewModel()
    {
        _vaultService = Defaults.Locator.GetRequiredService<VaultService>();
        _db = Defaults.Locator.GetRequiredService<KvExplorerDb>();
        Subscriptions = new ObservableCollection<SubscriptionDataItems>();
        Dispatcher.UIThread.InvokeAsync(async () =>
        {
            await GetAllKeyVaults();
        }, DispatcherPriority.SystemIdle);
    }

    /// <summary>
    /// The content of this page
    /// </summary>
    public string Message => "Press \"Next\" to register yourself.";

    /// <summary>
    /// The Title of this page
    /// </summary>
    public string Title => "Welcome to our Wizard-Sample.";

    [RelayCommand]
    public async Task GetAllKeyVaults()
    {
        int count = 0;
        await foreach (var item in _vaultService.GetAllSubscriptions())
        {
            Subscriptions.Add(new SubscriptionDataItems
            {
                Data = item.SubscriptionResource.Data,
                IsPinned = false
            });
            count++;
            if (item.ContinuationToken != null && count >= 100)
            {
                ContinuationToken = item.ContinuationToken;
                Debug.WriteLine(item.ContinuationToken);
                break;
            }
        }
        IsBusy = false;
    }
}