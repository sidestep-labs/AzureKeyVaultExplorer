﻿using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Extensions.Msal;
using System.Diagnostics;

namespace sidestep.quickey.Services;

public class AuthService
{
    public IPublicClientApplication authenticationClient;
    public MsalCacheHelper msalCacheHelper;
    // Providing the RedirectionUri to receive the token based on success or failure.

    public AuthService()
    {
        authenticationClient = PublicClientApplicationBuilder.Create(Constants.ClientId)
            //#if MACCATALYST
            //            .WithRedirectUri($"https://login.microsoftonline.com/common/oauth2/nativeclient")
            //#else
            .WithRedirectUri($"msal{Constants.ClientId}://auth")
            //#endif
            .WithIosKeychainSecurityGroup("com.companyname.sidestep.quickey")
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

            result = await authenticationClient
                    .AcquireTokenInteractive(Constants.Scopes)
#if ANDROID
                .WithParentActivityOrWindow(Microsoft.Maui.ApplicationModel.Platform.CurrentActivity)
#endif
                .ExecuteAsync(cancellationToken);

            // set the preferences/settings of the signed in account

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

        var account = await authenticationClient.GetAccountsAsync();
        if (!account.Any())
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

    public async Task MasCatalystAuthAsync()
    {
        try
        {


            //https://learn.microsoft.com/en-us/dotnet/maui/platform-integration/communication/authentication?view=net-maui-7.0&tabs=ios
            WebAuthenticatorResult authResult = await WebAuthenticator.Default.AuthenticateAsync(
                new WebAuthenticatorOptions()
                {

                    Url = new Uri("https://login.microsoftonline.com/common/oauth2/nativeclient"),
                    CallbackUrl = new Uri($"myapp://"),

                    //Url = new Uri("io.identitymodel.native://callback"),
                    //CallbackUrl = new Uri("https://demo.identityserver.io"),
                    //PrefersEphemeralWebBrowserSession = true
                });

            string accessToken = authResult?.AccessToken;

            // Do something with the token
        }
        catch (TaskCanceledException e)
        {
            // Use stopped auth
            Debug.WriteLine(e.Message);
        }
    }
}