using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Extensions.Msal;

namespace sidestep.quickey.Services;

public class AuthService
{
    public IPublicClientApplication authenticationClient;
    public MsalCacheHelper msalCacheHelper;
    // Providing the RedirectionUri to receive the token based on success or failure.

    public AuthService()
    {
        authenticationClient = PublicClientApplicationBuilder.Create(Constants.ClientId)
            .WithRedirectUri($"msal{Constants.ClientId}://auth")
            .Build();
    }

    // Propagates notification that the operation should be cancelled.
    public async Task<AuthenticationResult> LoginAsync(CancellationToken cancellationToken)
    {
        await AttachTokenCache();

        AuthenticationResult result;
        try
        {


            result = await authenticationClient
                .AcquireTokenInteractive(Constants.Scopes)
                //.WithPrompt(Prompt.ForceLogin) //This is optional. If provided, on each execution, the username and the password must be entered.
#if ANDROID
                .WithParentActivityOrWindow(Microsoft.Maui.ApplicationModel.Platform.CurrentActivity)
#endif



                .ExecuteAsync(cancellationToken);

            // set the preferences/settings of the signed in account
           
            //Preferences.Default.Set("auth_account_id", JsonSerializer.Serialize(result.UniqueId));
            return result;
        }
        catch (MsalClientException)
        {
            return null;
        }
    }

    /// <summary>
    /// returns the authenticated account with a refreshed token
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<AuthenticationResult> RefreshTokenAsync(CancellationToken cancellationToken)
    {
        await AttachTokenCache();
        AuthenticationResult authenticationResult;

        var account = await authenticationClient.GetAccountsAsync();
        if(!account.Any())
        {
            return null;
        }
        authenticationResult = await authenticationClient.AcquireTokenSilent(Constants.Scopes, account.FirstOrDefault()).WithForceRefresh(true).ExecuteAsync();

        return authenticationResult;
    }

    private async Task<IEnumerable<IAccount>> AttachTokenCache()
    {
        if (DeviceInfo.Current.Platform != DevicePlatform.WinUI)
        {
            return null;
        }

        // Cache configuration and hook-up to public application. Refer to https://github.com/AzureAD/microsoft-authentication-extensions-for-dotnet/wiki/Cross-platform-Token-Cache#configuring-the-token-cache
        //var storageProperties = new StorageCreationPropertiesBuilder("netcore_maui_broker_cache.txt", "C:/temp")

        var storageProperties =
             new StorageCreationPropertiesBuilder(Constants.CacheFileName, Constants.CacheDir)
               .WithLinuxKeyring(Constants.LinuxKeyRingSchema, Constants.LinuxKeyRingCollection, Constants.LinuxKeyRingLabel, Constants.LinuxKeyRingAttr1, Constants.LinuxKeyRingAttr2)
               .WithMacKeyChain(Constants.KeyChainServiceName, Constants.KeyChainAccountName)
               .Build();
        
        msalCacheHelper = await MsalCacheHelper.CreateAsync(storageProperties);
        msalCacheHelper.RegisterCache(authenticationClient.UserTokenCache);

        //msalCacheHelper.CacheChanged += (object sender, CacheChangedEventArgs args) =>
        //{
        //    Console.WriteLine($"Cache Changed, Added: {args.AccountsAdded.Count()} Removed: {args.AccountsRemoved.Count()}");
        //};

        // If the cache file is being reused, we'd find some already-signed-in accounts
        return await authenticationClient.GetAccountsAsync().ConfigureAwait(false);
    }
}