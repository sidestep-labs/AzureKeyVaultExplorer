namespace KeyVaultExplorer.Database;

public partial class BookmarkedItem
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string VaultUri { get; set; } = null!;

    public long Type { get; set; }

    public string? SubscriptionDisplayName { get; set; }

    public string? ContentType { get; set; }

    public string Version { get; set; } = null!;
}