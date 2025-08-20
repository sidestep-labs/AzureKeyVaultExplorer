using Azure.Security.KeyVault.Certificates;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KeyVaultExplorer.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KeyVaultExplorer.ViewModels;

public partial class EditCertificateVersionViewModel : ViewModelBase
{
    private readonly AuthService _authService;
    private readonly VaultService _vaultService;
    private readonly NotificationViewModel _notificationViewModel;

    [ObservableProperty]
    private TimeSpan? expiresOnTimespan;

    [ObservableProperty]
    private bool isBusy = false;

    [ObservableProperty]
    private CertificateProperties keyVaultCertificateModel = new CertificateProperties("");

    [ObservableProperty]
    private TimeSpan? notBeforeTimespan;

    [ObservableProperty]
    public bool hasActivationDateChecked;

    [ObservableProperty]
    public bool hasExpirationDateChecked;

    public EditCertificateVersionViewModel()
    {
        _authService = Defaults.Locator.GetRequiredService<AuthService>();
        _vaultService = Defaults.Locator.GetRequiredService<VaultService>();
        _notificationViewModel = Defaults.Locator.GetRequiredService<NotificationViewModel>();
    }

    // Delegate to get updated tags from the UI
    public Func<IDictionary<string, string>>? GetUpdatedTags { get; set; }

    public string? Identifier => KeyVaultCertificateModel?.Id?.ToString();
    public string? Location => KeyVaultCertificateModel?.VaultUri.ToString();

    [RelayCommand]
    public async Task EditDetails()
    {
        // For certificates, we can only update tags and enabled status
        // Date properties are typically managed by the certificate lifecycle
        
        // Update tags from the TagsEditor if available
        UpdateTagsFromEditor();

        var updatedCert = await _vaultService.UpdateCertificate(KeyVaultCertificateModel, KeyVaultCertificateModel.VaultUri);
        KeyVaultCertificateModel = updatedCert;
    }

    private void UpdateTagsFromEditor()
    {
        if (GetUpdatedTags != null)
        {
            var updatedTags = GetUpdatedTags();
            KeyVaultCertificateModel.Tags.Clear();
            foreach (var tag in updatedTags)
            {
                KeyVaultCertificateModel.Tags[tag.Key] = tag.Value;
            }
        }
    }

    partial void OnKeyVaultCertificateModelChanging(CertificateProperties value)
    {
        if (value != null)
        {
            HasActivationDateChecked = value.NotBefore.HasValue;
            HasExpirationDateChecked = value.ExpiresOn.HasValue;
            ExpiresOnTimespan = value.ExpiresOn.HasValue ? value.ExpiresOn.Value.LocalDateTime.TimeOfDay : null;
            NotBeforeTimespan = value.NotBefore.HasValue ? value.NotBefore.Value.LocalDateTime.TimeOfDay : null;
        }
    }

    partial void OnHasActivationDateCheckedChanged(bool oldValue, bool newValue)
    {
        // Certificate dates are usually managed automatically
        // This might not be applicable for certificates
    }

    partial void OnHasExpirationDateCheckedChanged(bool oldValue, bool newValue)
    {
        // Certificate dates are usually managed automatically  
        // This might not be applicable for certificates
    }
}