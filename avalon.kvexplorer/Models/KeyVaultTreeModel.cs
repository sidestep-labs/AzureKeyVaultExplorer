using Azure.ResourceManager.KeyVault;

namespace avalon.kvexplorer.Models;

public class KeyVaultTreeModel
{
    public string SubscriptionName { get; set; }

    public KeyVaultResource[] KeyVaultResources { get; set; }
}