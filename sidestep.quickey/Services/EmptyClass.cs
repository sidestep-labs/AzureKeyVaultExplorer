using System;
using Azure.Core;
using Microsoft.Identity.Client;

namespace sidestep.quickey.Services;

public class CustomTokenCredential : TokenCredential
{
    private readonly string _token;
    private readonly DateTimeOffset _expiresOn;
    public CustomTokenCredential(AuthenticationResult access)
    {
        _expiresOn = access.ExpiresOn;
        _token = access.AccessToken;
    }
    public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken)
    {
        return new AccessToken(_token, _expiresOn);
    }

    public override ValueTask<AccessToken> GetTokenAsync(TokenRequestContext requestContext, CancellationToken cancellationToken)
    {
        return ValueTask.FromResult(new AccessToken(_token, _expiresOn));
    }
}

