using Azure.ResourceManager.Resources;
using Azure.ResourceManager;
using Azure.ResourceManager.KeyVault;
namespace sidestep.quickey.Services;

public class VaultService
{

    public AuthService _authService { get; set; }
    public VaultService(AuthService authService)
    {
        _authService = authService;
    }





}