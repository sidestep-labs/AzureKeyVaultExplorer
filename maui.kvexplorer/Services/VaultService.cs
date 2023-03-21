using Azure.ResourceManager;
using Azure.ResourceManager.KeyVault;
using Azure.Security.KeyVault.Certificates;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Secrets;

namespace kvexplorer.Services;

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
        //SubscriptionResource subscription = await armClient.GetDefaultSubscriptionAsync();
        var subscriptions = armClient.GetSubscriptions().ToArray();
        foreach (var subscription in subscriptions)
        {
            await foreach (var kvResource in subscription.GetKeyVaultsAsync())
            {
                yield return kvResource;
            }
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