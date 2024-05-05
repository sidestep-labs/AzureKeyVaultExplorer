using System;
namespace KeyVaultExplorer.Database;

public class Subscriptions
{
    public required string  DisplayName { get; set; }
    public required string SubscriptionId { get; set; }
    public required Guid TenantId { get; set; }

}
