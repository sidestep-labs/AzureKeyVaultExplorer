using Azure.Security.KeyVault.Certificates;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Secrets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kvexplorer.shared.Models;

public class KeyVaultContentsAmalgamation
{
    public KeyVaultItemType Type { get; set; }
    public required Uri Id { get; set; }
    public required Uri VaultUri { get; set; }
    public required string Name { get; set; }
    public required string Version { get; set; }
    public required string ContentType { get; set; }

    public SecretProperties SecretProperties { get; set; } = null!;

    public KeyProperties KeyProperties { get; set; } = null!;

    public CertificateProperties? CertificateProperties { get; set; } = null!;
}

public enum KeyVaultItemType
{
    Certificate,
    Secret,
    Key
}