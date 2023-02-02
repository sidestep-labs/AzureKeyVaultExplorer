namespace sidestep.quickey;

public static class Constants
{
    //The Application or Client ID will be generated while registering the app in the Azure portal. Copy and paste the GUID.
    public static readonly string ClientId = "7c09c1d9-3585-403c-834a-53452958e76f";

    //Leaving the scope to its default values.
    public static readonly string[] Scopes = new string[] { "openid", "offline_access" };
}