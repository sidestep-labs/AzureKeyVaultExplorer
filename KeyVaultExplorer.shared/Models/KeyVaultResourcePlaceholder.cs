using Azure.ResourceManager.KeyVault;

namespace KeyVaultExplorer.shared;

public partial class VaultService
{
    // needed to make the tree
    public class KeyVaultResourcePlaceholder : KeyVaultResource
    { }
}