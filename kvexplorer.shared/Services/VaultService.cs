using Azure.ResourceManager;
using Azure.ResourceManager.KeyVault;
using Azure.Security.KeyVault.Certificates;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Secrets;
using kvexplorer.shared.Models;

namespace kvexplorer.shared;

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

    public async IAsyncEnumerable<KeyVaultModel> GetKeyVaultResourceBySubscriptionAndResourceGroup()
    {
        var token = new CustomTokenCredential(await _authService.GetAzureArmTokenSilent());
        var armClient = new ArmClient(token);
        foreach (var subscription in armClient.GetSubscriptions().ToArray())
        {
            var resource = new KeyVaultModel
            {
                SubscriptionDisplayName = subscription.Data.DisplayName,
                SubscriptionId = subscription.Data.Id
            };
            await foreach (var kvResource in subscription.GetKeyVaultsAsync())
            {
                resource.KeyVaultResources.Add(kvResource);
            }

            yield return resource;
        }
    }

    public async IAsyncEnumerable<KeyProperties> GetVaultAssociatedKeys(Uri KvUri)
    {
        var token = new CustomTokenCredential(await _authService.GetAzureKeyVaultTokenSilent());
        var client = new KeyClient(KvUri, token);
        await foreach (var keyProperties in client.GetPropertiesOfKeysAsync())
        {
            await Console.Out.WriteLineAsync(keyProperties.Name);
            yield return keyProperties;
        }
    }

    public async IAsyncEnumerable<SecretProperties> GetVaultAssociatedSecrets(Uri KvUri)
    {
        var token = new CustomTokenCredential(await _authService.GetAzureKeyVaultTokenSilent());
        var client = new SecretClient(KvUri, token);
        await foreach (var secretProperties in client.GetPropertiesOfSecretsAsync())
        {
            await Console.Out.WriteLineAsync(secretProperties.Name);
            yield return secretProperties;
        }
    }

    public async IAsyncEnumerable<CertificateProperties> GetVaultAssociatedCertificates(Uri KvUri)
    {
        var token = new CustomTokenCredential(await _authService.GetAzureKeyVaultTokenSilent());
        var client = new CertificateClient(KvUri, token);
        await foreach (var certProperties in client.GetPropertiesOfCertificatesAsync())
        {
            await Console.Out.WriteLineAsync(certProperties.Name);
            yield return certProperties;
        }
    }
}