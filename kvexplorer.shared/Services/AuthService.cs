using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Extensions.Msal;
using System.Diagnostics;

namespace kvexplorer.shared;

public class AuthService
{
    public IPublicClientApplication authenticationClient;
    public MsalCacheHelper msalCacheHelper;

    // Providing the RedirectionUri to receive the token based on success or failure.

    public AuthService()
    {
        authenticationClient = PublicClientApplicationBuilder.Create(Constants.ClientId)
            .WithRedirectUri($"msal{Constants.ClientId}://auth")
            .WithRedirectUri("http://localhost")
            .WithIosKeychainSecurityGroup("com.sidestep.kvexplorer")
            .Build();
    }

    // Propagates notification that the operation should be cancelled.
    public async Task<AuthenticationResult> LoginAsync(CancellationToken cancellationToken)
    {
        await AttachTokenCache();

        AuthenticationResult result;
        try
        {
            var options = new SystemWebViewOptions()
            {
                HtmlMessageError = "<p> An error occured: {0}. Details {1}</p>",
                BrowserRedirectSuccess = new Uri("https://www.microsoft.com")
            };

            //.WithPrompt(Prompt.ForceLogin) //This is optional. If provided, on each execution, the username and the password must be entered.
            //#if MACCATALYST
            //.WithUseEmbeddedWebView(false)
            //.WithSystemWebViewOptions(options)
            //#endif

            result = await authenticationClient.AcquireTokenInteractive(Constants.Scopes).WithExtraScopesToConsent(Constants.AzureRMScope).ExecuteAsync(cancellationToken);

            // set the preferences/settings of the signed in account
            //IAccount cachedUserAccount = Task.Run(async () => await PublicClientSingleton.Instance.MSALClientHelper.FetchSignedInUserFromCache()).Result;

            //Preferences.Default.Set("auth_account_id", JsonSerializer.Serialize(result.UniqueId));

            return result;
        }
        catch (MsalClientException ex)
        {
            Debug.WriteLine(ex);
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

        var accounts = await authenticationClient.GetAccountsAsync();

        if (!accounts.Any())
        {
            return null;
        }

        var account = accounts.First();

        authenticationResult = await authenticationClient.AcquireTokenSilent(Constants.Scopes, accounts.FirstOrDefault()).WithForceRefresh(true).ExecuteAsync();

        return authenticationResult;
    }

    private async Task<IEnumerable<IAccount>> AttachTokenCache()
    {
        // Cache configuration and hook-up to public application. Refer to https://github.com/AzureAD/microsoft-authentication-extensions-for-dotnet/wiki/Cross-platform-Token-Cache#configuring-the-token-cache
        //var storageProperties = new StorageCreationPropertiesBuilder("netcore_maui_broker_cache.txt", "C:/temp")

        var storageProperties =
             new StorageCreationPropertiesBuilder(Constants.CacheFileName, Constants.LocalAppDataFolder)
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

    public async Task RemoveAccount()
    {
        await AttachTokenCache();
        var accounts = await authenticationClient.GetAccountsAsync();
        await authenticationClient.RemoveAsync(accounts.FirstOrDefault());
    }

    public async Task<AuthenticationResult> GetAzureArmTokenSilent()
    {
        await AttachTokenCache();
        var accounts = await authenticationClient.GetAccountsAsync();
        if (!accounts.Any())
        {
            await LoginAsync(CancellationToken.None);
            accounts = await authenticationClient.GetAccountsAsync();
        }
        return await authenticationClient.AcquireTokenSilent(Constants.AzureRMScope, accounts.First()).ExecuteAsync();
    }

    public async Task<AuthenticationResult?> GetAzureKeyVaultTokenSilent()
    {
        await AttachTokenCache();
        var accounts = await authenticationClient.GetAccountsAsync();
        return await authenticationClient.AcquireTokenSilent(Constants.KvScope, accounts.First()).ExecuteAsync();
    }
}