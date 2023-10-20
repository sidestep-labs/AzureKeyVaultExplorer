using Azure.ResourceManager.Resources.Models;
using System;
using System.Collections.Generic;

namespace kvexplorer.shared.Database;

public partial class QuickAccess
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string VaultUri { get; set; } = null!;

    public string KeyVaultId { get; set; } = null!;

    public string? SubscriptionDisplayName { get; set; }

    public string? SubscriptionId { get; set; }

    public required string TenantId { get; set; }

    public string Location { get; set; } = null!;
}
