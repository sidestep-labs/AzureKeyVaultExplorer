using Microsoft.Identity.Client.Extensions.Msal;
using System;
using System.Collections.Generic;

namespace KeyVaultExplorer.Models;

public static class Constants
{
    //The Application or Client ID will be generated while registering the app in the Azure portal. Copy and paste the GUID.
    public static readonly string ClientId = "7c09c1d9-3585-403c-834a-53452958e76f";

    //public static readonly Uri Url = new Uri($"https://login.microsoftonline.com/common/adminconsent?client_id={ClientId}&state=initlogin&redirect_uri=msauth.com.company.KeyVaultExplorer://auth");

    //public static readonly Uri Url = new Uri($"https://login.microsoftonline.com/common/oauth2/v2.0/authorize?client_id={ClientId}&scope=https%3A%2F%2Fgraph.microsoft.com%2F.default&grant_type=client_credentials&redirect_uri=msauth.com.company.KeyVaultExplorer://auth");

    public static readonly Uri Url = new($"https://login.microsoftonline.com/958763de-a224-4d6c-8eb1-c63b2fb66f1b/oauth2/v2.0/authorize?" +
        $"client_id={ClientId}" +
        $"&response_type=token code" +
        $"&redirect_uri=msauth.com.company.KeyVaultExplorer://auth" +
        $"&response_mode=query" +
        $"&scope=openid offline_access profile email" +
        $"&state=login" +
        $"&nonce=111821");

    public static readonly Uri AuthCodeFlowUri = new($"https://login.microsoftonline.com/common/oauth2/v2.0/authorize" +
        $"?client_id={ClientId}" +
        $"&response_type=code id_token" +
        $"&redirect_uri=msauth.com.company.KeyVaultExplorer://auth" +
        $"&response_mode=fragment" +
        $"&scope=openid offline_access profile email" +
        $"&state=1" +
        $"&nonce=1");

    //$"&code_challenge=YTFjNjI1OWYzMzA3MTI4ZDY2Njg5M2RkNmVjNDE5YmEyZGRhOGYyM2IzNjdmZWFhMTQ1ODg3NDcxY2Nl" +
    //$"&code_challenge_method=S25

    public static readonly Uri AuthCodeFlowAccessTokenUri = new($"https://login.microsoftonline.com/common/oauth2/v2.0/authorize" +
           $"?client_id={ClientId}" +
           $"&response_type=code id_token" +
           $"&redirect_uri=msauth.com.company.KeyVaultExplorer://auth" +
           $"&response_mode=fragment" +
           $"&scope=openid offline_access profile email" +
           $"&state=1" +
           $"&nonce=1");

    //Leaving the scope to its default values.
    public static readonly string[] Scopes = ["openid", "offline_access", "profile", "email",];

    public static readonly string[] AzureRMScope = ["https://management.core.windows.net//.default"];

    public static readonly string[] KvScope = ["https://vault.azure.net/.default"];

    public static readonly string[] AzureScopes = ["https://management.core.windows.net//.default", "https://vault.azure.net//.default", "user_impersonation"];

    // Cache settings
    public const string CacheFileName = "kvexplorer_msal_cache.txt";

    public static readonly string LocalAppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\KeyVaultExplorer";

    public const string KeyChainServiceName = "keyvaultexplorer_msal_service";
    public const string KeyChainAccountName = "keyvaultexplorer_msal_account";

    public const string LinuxKeyRingSchema = "com.sidestep.keyvaultexplorer.tokencache";
    public const string LinuxKeyRingCollection = MsalCacheHelper.LinuxKeyRingDefaultCollection;
    public const string LinuxKeyRingLabel = "MSAL token cache for all Contoso dev tool apps.";
    public static readonly KeyValuePair<string, string> LinuxKeyRingAttr1 = new KeyValuePair<string, string>("Version", "1");
    public static readonly KeyValuePair<string, string> LinuxKeyRingAttr2 = new KeyValuePair<string, string>("ProductGroup", "MyApps");
}