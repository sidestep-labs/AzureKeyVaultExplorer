using Azure.ResourceManager.KeyVault;
using System.Collections.ObjectModel;

namespace avalon.kvexplorer.Models;

public class KeyVaultModel
{
    public string SubscriptionDisplayName { get; set; }

    public string SubscriptionId { get; set; }

    public ObservableCollection<KeyVaultResource> KeyVaultResources { get; }  = new ObservableCollection<KeyVaultResource>();
}