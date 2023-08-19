using Azure.ResourceManager.KeyVault;
using CommunityToolkit.Mvvm.ComponentModel;

namespace kvexplorer.shared.Models;

public partial class KeyVaultModel : ObservableObject
{
    public string? SubscriptionDisplayName { get; set; }

    public string? SubscriptionId { get; set; }

    public List<KeyVaultResource> KeyVaultResources { get; set; } = new List<KeyVaultResource>();

    [ObservableProperty]
    private bool isExpanded;

}