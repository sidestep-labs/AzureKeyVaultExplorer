namespace sidestep.quickey;

public static class Constants
{
    //The Application or Client ID will be generated while registering the app in the Azure portal. Copy and paste the GUID.
    public static readonly string ClientId = "af5aa517-b812-4e07-ae81-bbc03410670e";

    //Leaving the scope to its default values.
    public static readonly string[] Scopes = new string[] { "openid", "offline_access" };
}