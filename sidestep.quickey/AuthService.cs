using Microsoft.Identity.Client;
using System.Text.Json;

namespace sidestep.quickey;

public class AuthService
{
    private readonly IPublicClientApplication authenticationClient;

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
        AuthenticationResult result;
        try
        {
            result = await authenticationClient
                .AcquireTokenInteractive(Constants.Scopes)
                //.WithPrompt(Prompt.ForceLogin) //This is optional. If provided, on each execution, the username and the password must be entered.
                .ExecuteAsync(cancellationToken);

            // set the preferences/settings of the signed in account
            Preferences.Default.Set("account", JsonSerializer.Serialize(result.Account));

            return result;
        }
        catch (MsalClientException)
        {
            return null;
        }
    }

    public async Task<AuthenticationResult> RefreshTokenAsync(CancellationToken cancellationToken)
    {
        AuthenticationResult result;
        string accountJson = Preferences.Default.Get("account", "");
        IAccount account;
        if (string.IsNullOrWhiteSpace(accountJson))
        {
            var res = await LoginAsync(CancellationToken.None);
            account = res.Account;
            //throw new ArgumentNullException("Account was not saved in preferences and the refresh token was called");
        }
        else
        {
            account = JsonSerializer.Deserialize<IAccount>(accountJson);
        }

        try
        {
            result = await authenticationClient.AcquireTokenSilent(Constants.Scopes, account).WithForceRefresh(true).ExecuteAsync();
            return result;
        }
        catch (MsalClientException)
        {
            return null;
        }
    }
}