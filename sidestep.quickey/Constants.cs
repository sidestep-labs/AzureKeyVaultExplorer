using Microsoft.Identity.Client.Extensions.Msal;

namespace sidestep.quickey;

public static class Constants
{
    //The Application or Client ID will be generated while registering the app in the Azure portal. Copy and paste the GUID.
    public static readonly string ClientId = "7c09c1d9-3585-403c-834a-53452958e76f";
    //public static readonly Uri Url = new Uri($"https://login.microsoftonline.com/common/adminconsent?client_id={ClientId}&state=initlogin&redirect_uri=msauth.com.company.sidestep.quickey://auth");
    
    public static readonly Uri Url = new Uri($"https://login.microsoftonline.com/common/oauth2/v2.0/authorize?client_id={ClientId}&scope=https%3A%2F%2Fgraph.microsoft.com%2F.default&redirect_uri=msauth.com.company.sidestep.quickey://auth");




    //Leaving the scope to its default values.
    public static readonly string[] Scopes = new string[] { "openid", "offline_access", };


    public static readonly string[] AzureRMScope = new string[] { "https://management.core.windows.net//.default"};

    public static readonly string[] KvScope = new string[] {  "https://vault.azure.net//.default" };

    public static readonly string[] AzureScopes = new string[] { "https://management.core.windows.net//.default",  "https://vault.azure.net//.default" };


    // Cache settings
    public const string CacheFileName = "quickey_msal_cache.txt";
    public readonly static string CacheDir = FileSystem.Current.CacheDirectory;

    public const string KeyChainServiceName = "quickey_msal_service";
    public const string KeyChainAccountName = "quickey_msal_account";

    public const string LinuxKeyRingSchema = "com.contoso.devtools.tokencache";
    public const string LinuxKeyRingCollection = MsalCacheHelper.LinuxKeyRingDefaultCollection;
    public const string LinuxKeyRingLabel = "MSAL token cache for all Contoso dev tool apps.";
    public static readonly KeyValuePair<string, string> LinuxKeyRingAttr1 = new KeyValuePair<string, string>("Version", "1");
    public static readonly KeyValuePair<string, string> LinuxKeyRingAttr2 = new KeyValuePair<string, string>("ProductGroup", "MyApps");

}