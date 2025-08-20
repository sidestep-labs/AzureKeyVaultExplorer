namespace KeyVaultExplorer.Models;

public enum AzureCloud
{
    Public,
    USGovernment
}

public class CloudEnvironment
{
    public AzureCloud Cloud { get; set; }
    public string Name { get; set; }
    public string Authority { get; set; }
    public string VaultScope { get; set; }
    public string ManagementScope { get; set; }
    public string PortalUrl { get; set; }

    public static readonly CloudEnvironment Public = new()
    {
        Cloud = AzureCloud.Public,
        Name = "Azure Public Cloud",
        Authority = "https://login.microsoftonline.com",
        VaultScope = "https://vault.azure.net/.default",
        ManagementScope = "https://management.core.windows.net/.default",
        PortalUrl = "https://portal.azure.com"
    };

    public static readonly CloudEnvironment USGovernment = new()
    {
        Cloud = AzureCloud.USGovernment,
        Name = "Azure US Government",
        Authority = "https://login.microsoftonline.us",
        VaultScope = "https://vault.usgovcloudapi.net/.default",
        ManagementScope = "https://management.usgovcloudapi.net/.default",
        PortalUrl = "https://portal.azure.us"
    };

    public static CloudEnvironment GetCloudEnvironment(AzureCloud cloud)
    {
        return cloud switch
        {
            AzureCloud.Public => Public,
            AzureCloud.USGovernment => USGovernment,
            _ => Public
        };
    }

    public static CloudEnvironment[] AllEnvironments => [Public, USGovernment];
}