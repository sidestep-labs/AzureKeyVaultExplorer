using Avalonia.Threading;
using Azure.ResourceManager.Resources;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using kvexplorer.shared;
using kvexplorer.shared.Database;
using kvexplorer.shared.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace kvexplorer.ViewModels;

public partial class BookmarksPageViewModel : ViewModelBase
{
    [ObservableProperty]
    public bool isBusy = true;

    [ObservableProperty]
    public ObservableCollection<SubscriptionResource> selectedSubscriptions;

    [ObservableProperty]
    public ObservableCollection<SubscriptionData> subscriptions;


    [ObservableProperty]
    public string continuationToken; 

    private readonly KvExplorerDb _db;
    private readonly VaultService _vaultService;
    public BookmarksPageViewModel()
    {
        _vaultService = Defaults.Locator.GetRequiredService<VaultService>();
        _db = Defaults.Locator.GetRequiredService<KvExplorerDb>();
        SelectedSubscriptions = new();
        Subscriptions = new ObservableCollection<SubscriptionData>();
        GetAllKeyVaults().Wait();
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
        await Dispatcher.UIThread.InvokeAsync(async () =>
        {
            var resource = _vaultService.GetAllSubscriptions();
            await foreach (var item in resource)
            {
                Subscriptions.Add(item.SubscriptionResource.Data);
                count++;
                if (item.ContinuationToken != null && count >= 100)
                {
                    ContinuationToken = item.ContinuationToken;
                    Debug.WriteLine(item.ContinuationToken);
                    break;
                }
            }
            IsBusy = false;
        });
    }
}