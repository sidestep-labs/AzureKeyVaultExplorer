using Azure.ResourceManager.KeyVault;
using Azure.ResourceManager.Resources;
using CommunityToolkit.Mvvm.ComponentModel;

namespace kvexplorer.shared.Models;

public partial class KeyVaultModel : ObservableObject
{
    public string? SubscriptionDisplayName { get; set; }
    
    public required SubscriptionResource Subscription { get; set; } = null!;

    public string? SubscriptionId { get; set; }

    public List<KeyVaultResource> KeyVaultResources { get; set; } = new List<KeyVaultResource>() { };

    [ObservableProperty]
    private bool isExpanded;

    [ObservableProperty]
    private bool isSelected;
}