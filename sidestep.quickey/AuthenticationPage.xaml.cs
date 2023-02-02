using CommunityToolkit.Maui.Alerts;
using Microsoft.Identity.Client;
using sidestep.quickey.Services;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Xml.Schema;

namespace sidestep.quickey;

public partial class AuthenticationPage : ContentPage
{
    private int count = 0;
    private JwtSecurityToken tokenValue;
    private string t;

    private AuthService _auth;
    public AuthenticationPage(AuthService auth)
    {
        InitializeComponent();
        _auth = auth;
    }

    //public AuthService authService => new AuthService();

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        try
        {
            AuthenticationResult authenticationResult;
            string token;
            var existingAccount = await _auth.RefreshTokenAsync(CancellationToken.None);
            
            if (existingAccount != null)
            {
                authenticationResult = existingAccount;
                token = authenticationResult?.IdToken; // AccessToken also can be used
            }
            else
            {
                var result = await _auth.LoginAsync(CancellationToken.None);
                token = result?.IdToken;
            }

            if (token != null)
            {
                var handler = new JwtSecurityTokenHandler();
                var data = handler.ReadJwtToken(token);
                var claims = data.Claims.ToList();
                if (data != null)
                {
                    var stringBuilder = new StringBuilder();
                    tokenValue = data;
                    stringBuilder.AppendLine($"Name: {data.Claims.FirstOrDefault(x => x.Type.Equals("name"))?.Value}");
                    stringBuilder.AppendLine($"Email: {data.Claims.FirstOrDefault(x => x.Type.Equals("preferred_username"))?.Value}");
                    await Toast.Make(stringBuilder.ToString()).Show();
                }
                Preferences.Set("is_authenticated", true);
                await Shell.Current.GoToAsync("..");

            }
        }
        catch (MsalClientException ex)
        {
            await Toast.Make(ex.Message).Show();
        }
    }

}