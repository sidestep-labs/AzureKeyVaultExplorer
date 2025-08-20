using Azure.Security.KeyVault.Keys;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KeyVaultExplorer.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KeyVaultExplorer.ViewModels;

public partial class EditKeyVersionViewModel : ViewModelBase
{
    private readonly AuthService _authService;
    private readonly VaultService _vaultService;
    private readonly NotificationViewModel _notificationViewModel;

    [ObservableProperty]
    private TimeSpan? expiresOnTimespan;

    [ObservableProperty]
    private bool isBusy = false;

    [ObservableProperty]
    private KeyProperties keyVaultKeyModel = new KeyProperties("");

    [ObservableProperty]
    private TimeSpan? notBeforeTimespan;

    [ObservableProperty]
    public bool hasActivationDateChecked;

    [ObservableProperty]
    public bool hasExpirationDateChecked;

    public EditKeyVersionViewModel()
    {
        _authService = Defaults.Locator.GetRequiredService<AuthService>();
        _vaultService = Defaults.Locator.GetRequiredService<VaultService>();
        _notificationViewModel = Defaults.Locator.GetRequiredService<NotificationViewModel>();
    }

    // Delegate to get updated tags from the UI
    public Func<IDictionary<string, string>>? GetUpdatedTags { get; set; }

    public string? Identifier => KeyVaultKeyModel?.Id?.ToString();
    public string? Location => KeyVaultKeyModel?.VaultUri.ToString();

    [RelayCommand]
    public async Task EditDetails()
    {
        if (KeyVaultKeyModel.NotBefore.HasValue && HasActivationDateChecked)
            KeyVaultKeyModel.NotBefore = KeyVaultKeyModel.NotBefore.Value.Date + (NotBeforeTimespan ?? TimeSpan.Zero);
        else
            KeyVaultKeyModel.NotBefore = null;

        if (KeyVaultKeyModel.ExpiresOn.HasValue && HasExpirationDateChecked)
            KeyVaultKeyModel.ExpiresOn = KeyVaultKeyModel.ExpiresOn.Value.Date + (ExpiresOnTimespan ?? TimeSpan.Zero);
        else
            KeyVaultKeyModel.ExpiresOn = null;

        // Update tags from the TagsEditor if available
        UpdateTagsFromEditor();

        var updatedKey = await _vaultService.UpdateKey(KeyVaultKeyModel, KeyVaultKeyModel.VaultUri);
        KeyVaultKeyModel = updatedKey.Properties;
    }

    private void UpdateTagsFromEditor()
    {
        if (GetUpdatedTags != null)
        {
            var updatedTags = GetUpdatedTags();
            KeyVaultKeyModel.Tags.Clear();
            foreach (var tag in updatedTags)
            {
                KeyVaultKeyModel.Tags[tag.Key] = tag.Value;
            }
        }
    }

    partial void OnKeyVaultKeyModelChanging(KeyProperties value)
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
        if (newValue is false)
        {
            KeyVaultKeyModel.NotBefore = null;
        }
    }

    partial void OnHasExpirationDateCheckedChanged(bool oldValue, bool newValue)
    {
        if (newValue is false)
        {
            KeyVaultKeyModel.ExpiresOn = null;
        }
    }
}