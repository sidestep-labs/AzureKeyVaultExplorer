namespace kvexplorer.shared.Database;

public class Subscriptions
{
    public required string  DisplayName { get; set; }
    public required string SubscriptionId { get; set; }
    public required Guid TenantId { get; set; }

}
