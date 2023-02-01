using Microsoft.Identity.Client;

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
                .WithPrompt(Prompt.ForceLogin) //This is optional. If provided, on each execution, the username and the password must be entered.
                .ExecuteAsync(cancellationToken);
            return result;
        }
        catch (MsalClientException)
        {
            return null;
        }
    }
}