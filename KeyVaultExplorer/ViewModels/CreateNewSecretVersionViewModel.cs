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
using System.Collections.Generic;
using KeyVaultExplorer.Models;
using System.Collections.ObjectModel;
using Avalonia.Threading;
using System.Diagnostics;
using Azure.ResourceManager.KeyVault;

namespace KeyVaultExplorer.ViewModels;

public partial class CreateNewSecretVersionViewModel : ViewModelBase
{
    [ObservableProperty]
    public List<KvResourceGroupModel> resourceGroupItems;

    private readonly AuthService _authService;

    private readonly VaultService _vaultService;

    private NotificationViewModel _notificationViewModel;

    [ObservableProperty]
    private TimeSpan? expiresOnTimespan;

    [ObservableProperty]
    private bool isBusy = false;

    [ObservableProperty]
    private bool isEdit = false;

    [ObservableProperty]
    private bool isNew = false;

    [ObservableProperty]
    private Uri vaultUri;


    [ObservableProperty]
    private ObservableCollection<KeyVaultResource> keyVaultResources = [];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Location))]
    [NotifyPropertyChangedFor(nameof(HasActivationDate))]
    [NotifyPropertyChangedFor(nameof(HasExpirationDate))]
    private SecretProperties keyVaultSecretModel;

    [ObservableProperty]
    private TimeSpan? notBeforeTimespan;

    [ObservableProperty]
    private string secretValue;


    [ObservableProperty]
    private string secretName;

    partial void OnKeyVaultSecretModelChanged(SecretProperties value)
    {
        SecretName = value.Name;
    }

    [ObservableProperty]
    private SubscriptionDataItem selectedSubscription;

    [ObservableProperty]
    private KeyVaultResource selectedKeyVault;

    [ObservableProperty]
    private ObservableCollection<SubscriptionDataItem> subscriptions;

    public CreateNewSecretVersionViewModel()
    {
        _authService = Defaults.Locator.GetRequiredService<AuthService>();
        _vaultService = Defaults.Locator.GetRequiredService<VaultService>();
        _notificationViewModel = Defaults.Locator.GetRequiredService<NotificationViewModel>();
    }

    public bool HasActivationDate => KeyVaultSecretModel is not null && KeyVaultSecretModel.NotBefore.HasValue;

    public bool HasExpirationDate => KeyVaultSecretModel is not null && KeyVaultSecretModel.ExpiresOn.HasValue;

    public string? Identifier => KeyVaultSecretModel?.Id?.ToString();

    public string? Location => KeyVaultSecretModel?.VaultUri.ToString();

    [RelayCommand]
    public async Task EditDetails()
    {
        if (KeyVaultSecretModel.NotBefore.HasValue)
            KeyVaultSecretModel.NotBefore = KeyVaultSecretModel.NotBefore.Value.Date + (NotBeforeTimespan ?? TimeSpan.Zero);

        if (KeyVaultSecretModel.ExpiresOn.HasValue)
            KeyVaultSecretModel.ExpiresOn = KeyVaultSecretModel.ExpiresOn.Value.Date + (ExpiresOnTimespan ?? TimeSpan.Zero);

        //foreach (var tag in KeyVaultSecretModel.Tags)
        //    KeyVaultSecretModel.Properties.Tags.Add(tag.Key, tag.Value);

        var updatedProps = await _vaultService.UpdateSecret(KeyVaultSecretModel, KeyVaultSecretModel.VaultUri);
        KeyVaultSecretModel = updatedProps;
    }

    public async Task<ObservableCollection<SubscriptionDataItem>> GetAvailableSubscriptions()
    {
        var subscriptions = new List<SubscriptionDataItem>();
        await foreach (var item in _vaultService.GetAllSubscriptions())
        {
            subscriptions.Add(new SubscriptionDataItem
            {
                Data = item.SubscriptionResource.Data,
                Resource = item.SubscriptionResource
            });
        }
        return new ObservableCollection<SubscriptionDataItem>(subscriptions);
    }

    [RelayCommand]
    public async Task NewVersion()
    {
        var newSecret = new KeyVaultSecret(SecretName ?? KeyVaultSecretModel.Name, SecretValue);
        if (KeyVaultSecretModel.NotBefore.HasValue)
            newSecret.Properties.NotBefore = KeyVaultSecretModel.NotBefore.Value.Date + (NotBeforeTimespan ?? TimeSpan.Zero);

        if (KeyVaultSecretModel.ExpiresOn.HasValue)
            newSecret.Properties.ExpiresOn = KeyVaultSecretModel.ExpiresOn.Value.Date + (ExpiresOnTimespan ?? TimeSpan.Zero);

        newSecret.Properties.ContentType = KeyVaultSecretModel.ContentType;

        foreach (var tag in KeyVaultSecretModel.Tags)
            newSecret.Properties.Tags.Add(tag.Key, tag.Value);

        var newVersion = await _vaultService.CreateSecret(newSecret, !IsNew ? KeyVaultSecretModel.VaultUri : VaultUri);
        var properties = (await _vaultService.GetSecretProperties(newVersion.Properties.VaultUri, newVersion.Name)).First();
        KeyVaultSecretModel = properties;
    }

    partial void OnKeyVaultSecretModelChanging(SecretProperties value)
    {
        ExpiresOnTimespan = value is not null && value.ExpiresOn.HasValue ? value?.ExpiresOn.Value.LocalDateTime.TimeOfDay : null;
        NotBeforeTimespan = value is not null && value.NotBefore.HasValue ? value?.NotBefore.Value.LocalDateTime.TimeOfDay : null;
    }

    private readonly string[] _seenSubscriptions = [];


    [RelayCommand]
    public void SelectedSubscriptionChanged(SubscriptionDataItem value)
    {
        if (value is not null && !_seenSubscriptions.Contains(value.Data.SubscriptionId))
        {
            Dispatcher.UIThread.Post(async () =>
            {
                var keyVaults = _vaultService.GetKeyVaultsBySubscription(new KvSubscriptionModel { Subscription = value.Resource });
                await foreach (var item in keyVaults)
                {
                    KeyVaultResources.Add(item);
                }
                _seenSubscriptions.Append(value.Data.SubscriptionId);
            }, DispatcherPriority.ApplicationIdle);
        }
    }
}