using Azure.ResourceManager.KeyVault;
using Azure.ResourceManager.Resources;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace kvexplorer.shared.Models;

public partial class KvSubscriptionModel : ObservableObject
{
    [ObservableProperty]
    private bool isExpanded;

    [ObservableProperty]
    private bool isSelected;

    public string? GlyphIcon { get; set; } = null;
    public List<KeyVaultResource> KeyVaultResources { get; set; } = [];
    public SubscriptionResource Subscription { get; set; } = null!;
    public string SubscriptionDisplayName { get; set; } = null!;
    public string? SubscriptionId { get; set; }
    public ObservableCollection<KvExplorerResourceGroup> ResourceGroups { get; set; } = [];
}

public partial class KvExplorerResourceGroup : ObservableObject
{
    public string? GlyphIcon { get; set; } = null;

    [ObservableProperty]
    private bool isExpanded;

    [ObservableProperty]
    private bool isSelected;
    public string ResourceGroupDisplayName { get; set; } = null!;
    public ResourceGroupResource ResourceGroupResource { get; set; } = null!;
    public ObservableCollection<KeyVaultResource> KeyVaultResources { get; set; } = [];

}