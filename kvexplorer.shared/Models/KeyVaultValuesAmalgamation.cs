using Azure.Security.KeyVault.Certificates;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Secrets;

namespace kvexplorer.shared.Models;

public class KeyVaultContentsAmalgamation
{
    public KeyVaultItemType Type { get; set; }
    public Uri Id { get; set; }
    public Uri VaultUri { get; set; }
    public string Name { get; set; }
    public string Version { get; set; }
    public string ContentType { get; set; }

    public SecretProperties SecretProperties { get; set; } = null!;

    public KeyProperties KeyProperties { get; set; } = null!;

    public CertificateProperties? CertificateProperties { get; set; } = null!;
}

public enum KeyVaultItemType
{
    Certificate,
    Secret,
    Key,
    All
}