using Azure.ResourceManager.KeyVault;

namespace kvexplorer.shared.Models;

public class KeyVaultModel
{
    public string? SubscriptionDisplayName { get; set; }

    public string? SubscriptionId { get; set; }

    public List<KeyVaultResource> KeyVaultResources { get; set; } = new List<KeyVaultResource>();
}

public class MyData
{
    public string Name { get; set; }
    public int Age { get; set; }
    public string Address { get; set; }
}