using Azure.Core;
using Azure.ResourceManager;
using Azure.ResourceManager.KeyVault;
using Azure.ResourceManager.KeyVault.Models;
using Azure.Security.KeyVault.Certificates;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Secrets;
using kvexplorer.shared.Models;

namespace kvexplorer.shared;
/* Call me a bad person for abstracting away/wrapping a library already doing all the work. */
public class VaultService
{
    public AuthService _authService { get; set; }

    public VaultService(AuthService authService)
    {
        _authService = authService;
    }

    public async IAsyncEnumerable<KeyVaultResource> GetKeyVaultResource()
    {
        var token = new CustomTokenCredential(await _authService.GetAzureArmTokenSilent());
        var armClient = new ArmClient(token);
        var subscription = await armClient.GetDefaultSubscriptionAsync();
        await foreach (var kvResource in subscription.GetKeyVaultsAsync())
        {
            yield return kvResource;
        }
    }

    public async IAsyncEnumerable<KeyVaultResource> GetKeyVaultResources()
    {
        var token = new CustomTokenCredential(await _authService.GetAzureArmTokenSilent());
        var armClient = new ArmClient(token);
        foreach (var subscription in armClient.GetSubscriptions().ToArray())
        {
            await foreach (var kvResource in subscription.GetKeyVaultsAsync())
            {
                yield return kvResource;
            }
        }
    }

    // needed to make the tree
    public class KeyVaultResourcePlaceholder : KeyVaultResource
    { }

    /// <summary>
    /// returns all key vaults based on all the subscriptions the user has rights to view.
    /// </summary>
    /// <returns></returns>
    public async IAsyncEnumerable<KeyVaultModel> GetKeyVaultResourceBySubscriptionAndResourceGroup()
    {
        var token = new CustomTokenCredential(await _authService.GetAzureArmTokenSilent());
        var armClient = new ArmClient(token);

        var placeholder = new KeyVaultResourcePlaceholder();
        foreach (var subscription in armClient.GetSubscriptions())
        {
            var resource = new KeyVaultModel
            {
                SubscriptionDisplayName = subscription.Data.DisplayName,
                SubscriptionId = subscription.Data.Id,
                Subscription = subscription,
                KeyVaultResources = new List<KeyVaultResource>() { placeholder }
            };
            yield return resource;
        }
    }

    public void UpdateSubscriptionWithKeyVaults(ref KeyVaultModel resource)
    {
        resource.KeyVaultResources.Clear();
        foreach (var kvResource in resource.Subscription.GetKeyVaults())
        {
            resource.KeyVaultResources.Add(kvResource);
        }
    }

    public IEnumerable<KeyVaultResource> GetKeyVaultsBySubscription(KeyVaultModel resource)
    {
        foreach (var kvResource in resource.Subscription.GetKeyVaults())
        {
            yield return kvResource;
        }
    }

    public async IAsyncEnumerable<KeyVaultResource> GetKeyVaultsBySubscriptionAsync(KeyVaultModel resource)
    {
        resource.KeyVaultResources.Clear();
        foreach (var kvResource in resource.Subscription.GetKeyVaults())
        {
            yield return kvResource;
        }
    }

    public async IAsyncEnumerable<KeyVaultResource> GetWithKeyVaultsBySubscriptionAsync(KeyVaultModel resource)
    {
        await foreach (var kvResource in resource.Subscription.GetKeyVaultsAsync())
        {
            yield return kvResource;
        }
    }

    public async IAsyncEnumerable<KeyProperties> GetVaultAssociatedKeys(Uri KvUri)
    {
        var token = new CustomTokenCredential(await _authService.GetAzureKeyVaultTokenSilent());
        var client = new KeyClient(KvUri, token);
        await foreach (var keyProperties in client.GetPropertiesOfKeysAsync())
        {
            yield return keyProperties;
        }
    }

    public async IAsyncEnumerable<SecretProperties> GetVaultAssociatedSecrets(Uri KvUri)
    {
        var token = new CustomTokenCredential(await _authService.GetAzureKeyVaultTokenSilent());
        var client = new SecretClient(KvUri, token);
        await foreach (var secretProperties in client.GetPropertiesOfSecretsAsync())
        {
            yield return secretProperties;
        }
    }

    public async IAsyncEnumerable<CertificateProperties> GetVaultAssociatedCertificates(Uri KvUri)
    {
        var token = new CustomTokenCredential(await _authService.GetAzureKeyVaultTokenSilent());
        var client = new CertificateClient(KvUri, token);
        await foreach (var certProperties in client.GetPropertiesOfCertificatesAsync())
        {
            yield return certProperties;
        }
    }

    public async Task<KeyVaultSecret> GetSecret(Uri KvUri, string secretName)
    {
        var token = new CustomTokenCredential(await _authService.GetAzureKeyVaultTokenSilent());
        var client = new SecretClient(KvUri, token);
        return await client.GetSecretAsync(secretName);
    }
}