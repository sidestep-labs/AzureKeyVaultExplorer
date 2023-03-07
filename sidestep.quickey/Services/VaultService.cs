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

    public async IAsyncEnumerable<KeyVaultResource> GetKeyVaultResources()
    {
        var token = new CustomTokenCredential(await _authService.GetAzureArmTokenSilent());
        var armClient = new ArmClient(token);
        SubscriptionResource subscription = await armClient.GetDefaultSubscriptionAsync();
        var kvResources = subscription.GetKeyVaultsAsync();

        await foreach (var kvResource in kvResources)
        {
            await Console.Out.WriteLineAsync(kvResource.Data.Name);
            yield return kvResource;
        }
    }





}