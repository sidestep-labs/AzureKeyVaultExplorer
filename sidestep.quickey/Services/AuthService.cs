using Azure.ResourceManager.Resources;
using Azure.ResourceManager;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Extensions.Msal;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Principal;
using System.Text.Json;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager;
using Azure.ResourceManager.KeyVault;
namespace sidestep.quickey.Services;

public class AuthService
{
    public IPublicClientApplication authenticationClient;
    public MsalCacheHelper msalCacheHelper;
    private static HttpClient _httpClient => new HttpClient();

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
                    .WithExtraScopesToConsent(Constants.AzureRMScope)
#if ANDROID
                .WithParentActivityOrWindow(Microsoft.Maui.ApplicationModel.Platform.CurrentActivity)
#endif
                .ExecuteAsync(cancellationToken);

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
        Preferences.Set("name", account.Username);
        Preferences.Set("email", account.Username);
        Preferences.Set("username", account.Username);
        Preferences.Set("is_authenticated", !string.IsNullOrEmpty(account.Username));

        authenticationResult = await authenticationClient.AcquireTokenSilent(Constants.Scopes, accounts.FirstOrDefault()).WithForceRefresh(true).ExecuteAsync();

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
        return await authenticationClient.AcquireTokenSilent(Constants.AzureRMScope, accounts.FirstOrDefault()).ExecuteAsync();
    }

    public async Task<AuthenticationResult> GetAzureKeyVaultTokenSilent()
    {

        await AttachTokenCache();
        var accounts = await authenticationClient.GetAccountsAsync();
        var res = await authenticationClient.AcquireTokenSilent(Constants.KvScope, accounts.FirstOrDefault()).ExecuteAsync();
        var tokenCredential = new CustomTokenCredential(res);

        var x = new SecretClient(new Uri("https://kv-quickey.vault.azure.net/"), tokenCredential); ;
        var test = x.GetSecret("test");
        return res;
    }

    #region MacCatalyst We bAuth Microsoft Identity Platform

    /// <summary>
    /// this is to be used for mac catalyst only
    /// </summary>
    /// <returns></returns>
    public async Task WebLoginAsync()
    {
        try
        {
            if (Preferences.Get("is_authenticated", false) == true)
            {
                await RefreshAccessTokenForAuthCodeFlow();
            }
            else
            {
                //https://learn.microsoft.com/en-us/dotnet/maui/platform-integration/communication/authentication?view=net-maui-7.0&tabs=ios
                WebAuthenticatorResult authResult = await WebAuthenticator.AuthenticateAsync(Constants.AuthCodeFlowUri,
                new Uri($"msauth.com.company.sidestep.quickey://auth")
            );
                await GetAccessTokenForAuthCodeFlow(authResult.Properties["code"]);

                var token = new JwtSecurityToken(jwtEncodedString: authResult.IdToken);
                var email = token.Claims.First(c => c.Type == "email").Value;
                var name = token.Claims.First(c => c.Type == "name").Value;
                Preferences.Set("name", name);
                Preferences.Set("email", email);
                Preferences.Set("username", name);
                Preferences.Set("is_authenticated", !string.IsNullOrEmpty(authResult.IdToken));
            }
        }
        catch (TaskCanceledException e)
        {
            // Use stopped auth
            Debug.WriteLine(e.Message);
        }
    }

    public async Task GetAccessTokenForAuthCodeFlow(string code)
    {
        var scopes = new string[] { "https://vault.azure.net/.default", "openid", "offline_access", "profile", "email" };

        var queryString = new Dictionary<string, string>
        {
            { "client_id", Constants.ClientId },
            { "scope", String.Join(" ", scopes) },
            { "code", code },
            { "redirect_uri", "msauth.com.company.sidestep.quickey://auth"},
            { "grant_type", "authorization_code" },
            //{ "code_verifier", "authorization_code" },
         };

        var request = await _httpClient.PostAsync("https://login.microsoftonline.com/common/oauth2/v2.0/token",
            new FormUrlEncodedContent(queryString));
        //https://learn.microsoft.com/en-us/azure/active-directory/develop/v2-oauth2-auth-code-flow#request-an-authorization-code
        var response = await JsonSerializer.DeserializeAsync<AuthenticationResponse>(await request.Content.ReadAsStreamAsync());
        Preferences.Default.Set("oauth_authentication_response", await request.Content.ReadAsStringAsync());

        await RefreshAccessTokenForAuthCodeFlow();
    }

    public async Task RefreshAccessTokenForAuthCodeFlow()
    {
        var scopes = new string[] { "https://vault.azure.net/.default", "openid", "offline_access", "profile", "email" };
        var cachedOAuth = Preferences.Default.Get("oauth_authentication_response", "");
        var auth = JsonSerializer.Deserialize<AuthenticationResponse>(cachedOAuth);
        var queryString = new Dictionary<string, string>
        {
            { "client_id", Constants.ClientId },
            { "scope", String.Join(" ",scopes) },
            { "refresh_token", auth.RefreshToken },
            { "redirect_uri", "msauth.com.company.sidestep.quickey://auth"},
            { "grant_type", "refresh_token" },
         };
        var request = await _httpClient.PostAsync("https://login.microsoftonline.com/common/oauth2/v2.0/token", new FormUrlEncodedContent(queryString));
        var response = await JsonSerializer.DeserializeAsync<AuthenticationResponse>(await request.Content.ReadAsStreamAsync());
        Preferences.Default.Set("oauth_authentication_response", await request.Content.ReadAsStringAsync());
    }

    #endregion MacCatalyst We bAuth Microsoft Identity Platform






    public async Task NewAzARMClient()
    {

        var armResult = await GetAzureArmTokenSilent();
        var token = new CustomTokenCredential(armResult);
        var armClient = new ArmClient(token);
        SubscriptionResource subscription = await armClient.GetDefaultSubscriptionAsync();
        var kvResources = subscription.GetKeyVaults(10);

        foreach(var kvResource in kvResources)
        {

            //var resource = armClient.GetKeyVaultResource(kvResource.Id);
            await Console.Out.WriteLineAsync(kvResource.Data.Name);

        }
    }





}