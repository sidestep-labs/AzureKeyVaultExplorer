using Azure.ResourceManager.Resources;
using CommunityToolkit.Mvvm.ComponentModel;

namespace KeyVaultExplorer.Models;

public partial class SubscriptionDataItem : ObservableObject
{
    public SubscriptionData Data { get; set; }

    [ObservableProperty]
    public bool isPinned;

    [ObservableProperty]
    public bool? isUpdated;

    //partial void OnIsPinnedChanged(bool value)
    //{
    //    Console.WriteLine($"Name has changed to {value}");
    //    isUpdated = value;
    //}

    // this sets the default value to make sure we're not tracking intitially loaded items, only changed.
    partial void OnIsPinnedChanging(bool oldValue, bool newValue)
    {
        if (IsUpdated is null)
            IsUpdated = false;
        else
            IsUpdated = true;
    }
}