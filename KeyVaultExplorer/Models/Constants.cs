using Microsoft.Identity.Client.Extensions.Msal;
using System;
using System.Collections.Generic;

namespace KeyVaultExplorer.Models;

public static class Constants
{
    //The Application or Client ID will be generated while registering the app in the Azure portal. Copy and paste the GUID.
    public static readonly string ClientId = "fdc1e6da-d735-4627-af3e-d40377f55713";

    //Leaving the scope to its default values.
    public static readonly string[] Scopes = ["openid", "offline_access", "profile", "email",];

    public static readonly string[] AzureRMScope = ["https://management.core.windows.net//.default"];

    public static readonly string[] KvScope = ["https://vault.azure.net/.default"];

    public static readonly string[] AzureScopes = ["https://management.core.windows.net//.default", "https://vault.azure.net//.default", "user_impersonation"];

    // Cache settings
    public const string CacheFileName = "keyvaultexplorer_msal_cache.txt";

    public static readonly string LocalAppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\KeyVaultExplorer";

    public const string KeyChainServiceName = "keyvaultexplorer_msal_service";
    public const string KeyChainAccountName = "keyvaultexplorer_msal_account";

    public const string LinuxKeyRingSchema = "com.sidestep.keyvaultexplorer.tokencache";
    public const string LinuxKeyRingCollection = MsalCacheHelper.LinuxKeyRingDefaultCollection;
    public const string LinuxKeyRingLabel = "MSAL token cache for key vault explorer.";
    public static readonly KeyValuePair<string, string> LinuxKeyRingAttr1 = new KeyValuePair<string, string>("Version", "1");
    public static readonly KeyValuePair<string, string> LinuxKeyRingAttr2 = new KeyValuePair<string, string>("ProductGroup", "MyApps");
}