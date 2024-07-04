using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KeyVaultExplorer.Views;
using KeyVaultExplorer.Services;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using Azure.Security.KeyVault.Secrets;
using System;

namespace KeyVaultExplorer.ViewModels;

public partial class CreateNewSecretVersionViewModel : ViewModelBase
{
    [ObservableProperty]
    private bool isBusy = false;

    [ObservableProperty]
    private bool isEdit = false;

    public bool HasActivationDate => KeyVaultSecretModel is not null && KeyVaultSecretModel.NotBefore.HasValue;
    public bool HasExpirationDate => KeyVaultSecretModel is not null && KeyVaultSecretModel.ExpiresOn.HasValue;

    [ObservableProperty]
    private string secretValue;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Location))]
    [NotifyPropertyChangedFor(nameof(HasActivationDate))]
    [NotifyPropertyChangedFor(nameof(HasExpirationDate))]
    private SecretProperties keyVaultSecretModel;

    [ObservableProperty]
    private TimeSpan? expiresOnTimespan;// => KeyVaultSecretModel is not null && KeyVaultSecretModel.ExpiresOn.HasValue ? KeyVaultSecretModel?.ExpiresOn.Value.LocalDateTime.TimeOfDay : null;

    [ObservableProperty]
    private TimeSpan? notBeforeTimespan;

    public string? Location => KeyVaultSecretModel?.VaultUri.ToString();
    public string? Identifier => KeyVaultSecretModel?.Id.ToString();

    private readonly AuthService _authService;
    private readonly VaultService _vaultService;
    private NotificationViewModel _notificationViewModel;

    public CreateNewSecretVersionViewModel()
    {
        _authService = Defaults.Locator.GetRequiredService<AuthService>();
        _vaultService = Defaults.Locator.GetRequiredService<VaultService>();
        _notificationViewModel = Defaults.Locator.GetRequiredService<NotificationViewModel>();
    }

    [RelayCommand]
    public async Task EditDetails()
    {

        if (KeyVaultSecretModel.NotBefore.HasValue)
        {
            var time = KeyVaultSecretModel.NotBefore.Value.Date + (NotBeforeTimespan.HasValue ? NotBeforeTimespan.Value : TimeSpan.Zero);
            KeyVaultSecretModel.NotBefore = KeyVaultSecretModel.NotBefore.Value.Date + (NotBeforeTimespan.HasValue ? NotBeforeTimespan.Value : TimeSpan.Zero);
        }

        if (KeyVaultSecretModel.ExpiresOn.HasValue)
            KeyVaultSecretModel.ExpiresOn = KeyVaultSecretModel.ExpiresOn.Value.Date + (ExpiresOnTimespan.HasValue ? ExpiresOnTimespan.Value : TimeSpan.Zero);

       var updatedProps =  await _vaultService.UpdateSecret(KeyVaultSecretModel, KeyVaultSecretModel.VaultUri);
        KeyVaultSecretModel = updatedProps;

    }

    [RelayCommand]
    public async Task NewVersion()
    {


        var newSecret = new KeyVaultSecret(KeyVaultSecretModel.Name, SecretValue);
        if (KeyVaultSecretModel.NotBefore.HasValue)
        {
            var time = KeyVaultSecretModel.NotBefore.Value.Date + (NotBeforeTimespan.HasValue ? NotBeforeTimespan.Value : TimeSpan.Zero);
            newSecret.Properties.NotBefore = KeyVaultSecretModel.NotBefore.Value.Date + (NotBeforeTimespan.HasValue ? NotBeforeTimespan.Value : TimeSpan.Zero);
        }

        if (KeyVaultSecretModel.ExpiresOn.HasValue)
            newSecret.Properties.ExpiresOn = KeyVaultSecretModel.ExpiresOn.Value.Date + (ExpiresOnTimespan.HasValue ? ExpiresOnTimespan.Value : TimeSpan.Zero);

        newSecret.Properties.ContentType = KeyVaultSecretModel.ContentType;
        newSecret.Properties.Tags = KeyVaultSecretModel.Tags;

        var newVersion = await _vaultService.CreateSecret(newSecret, KeyVaultSecretModel.VaultUri);
        var properties = (await _vaultService.GetSecretProperties(newVersion.Properties.VaultUri, newVersion.Name)).First();
        KeyVaultSecretModel = properties;


    }


    partial void OnKeyVaultSecretModelChanging(SecretProperties model)
    {
        ExpiresOnTimespan = model is not null && model.ExpiresOn.HasValue ? model?.ExpiresOn.Value.LocalDateTime.TimeOfDay : null;
        NotBeforeTimespan = model is not null && model.NotBefore.HasValue ? model?.NotBefore.Value.LocalDateTime.TimeOfDay : null;
    }


}