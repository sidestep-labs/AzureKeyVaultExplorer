using Azure.ResourceManager.KeyVault;
using Azure.ResourceManager.Resources;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Diagnostics;

namespace kvexplorer.shared.Models;

public partial class SubscriptionDataItem : ObservableObject
{
    public SubscriptionData Data { get; set; }

    [ObservableProperty]
    private bool isPinned;

    public bool IsUpdated { get; set; }

    partial void OnIsPinnedChanged(bool value)
    {
        Console.WriteLine($"Name has changed to {value}");
        IsUpdated = true;
    }
}