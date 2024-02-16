using Azure.ResourceManager.KeyVault;
using Azure.ResourceManager.Resources;
using CommunityToolkit.Mvvm.ComponentModel;

namespace kvexplorer.shared.Models;

public partial class SubscriptionDataItems : ObservableObject
{
    public SubscriptionData Data { get; set; }
    public bool IsPinned { get; set; }
}