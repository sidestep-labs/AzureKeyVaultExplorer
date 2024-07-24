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

namespace KeyVaultExplorer.ViewModels;

public partial class CreateNewSecretVersionViewModel : ViewModelBase
{
    [ObservableProperty]
    public List<KvResourceGroupModel> resourceGroupItems;

    private readonly AuthService _authService;

    private readonly VaultService _vaultService;

    private NotificationViewModel _notificationViewModel;

    private SubscriptionsPageViewModel _subscriptionsPageViewModel;

    [ObservableProperty]
    private TimeSpan? expiresOnTimespan;

    [ObservableProperty]
    private bool isBusy = false;

    [ObservableProperty]
    private bool isEdit = false;

    [ObservableProperty]
    private bool isNew = false;


    [ObservableProperty]
    private ObservableCollection<SubscriptionDataItem> subscriptions;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Location))]
    [NotifyPropertyChangedFor(nameof(HasActivationDate))]
    [NotifyPropertyChangedFor(nameof(HasExpirationDate))]
    private SecretProperties keyVaultSecretModel;


    [ObservableProperty]

    private ObservableCollection<string> testItems;


    [ObservableProperty]
    private TimeSpan? notBeforeTimespan;

    [ObservableProperty]
    private string secretValue;

    public CreateNewSecretVersionViewModel()
    {
        _authService = Defaults.Locator.GetRequiredService<AuthService>();
        _vaultService = Defaults.Locator.GetRequiredService<VaultService>();
        _notificationViewModel = Defaults.Locator.GetRequiredService<NotificationViewModel>();
        _subscriptionsPageViewModel = Defaults.Locator.GetRequiredService<SubscriptionsPageViewModel>();
        if (Subscriptions is null || Subscriptions.Count == 0)
        {
            Dispatcher.UIThread.InvokeAsync(async() => await _subscriptionsPageViewModel.GetSubscriptions(), DispatcherPriority.MaxValue);
            Subscriptions = _subscriptionsPageViewModel.Subscriptions;
        }
    }

    public bool HasActivationDate => KeyVaultSecretModel is not null && KeyVaultSecretModel.NotBefore.HasValue;
    public bool HasExpirationDate => KeyVaultSecretModel is not null && KeyVaultSecretModel.ExpiresOn.HasValue;
    public string? Identifier => KeyVaultSecretModel?.Id?.ToString();
    public string? Location => KeyVaultSecretModel?.VaultUri.ToString();



    [RelayCommand]
    public async Task EditDetails()
    {
        if (KeyVaultSecretModel.NotBefore.HasValue)
            KeyVaultSecretModel.NotBefore = KeyVaultSecretModel.NotBefore.Value.Date + (NotBeforeTimespan.HasValue ? NotBeforeTimespan.Value : TimeSpan.Zero);

        if (KeyVaultSecretModel.ExpiresOn.HasValue)
            KeyVaultSecretModel.ExpiresOn = KeyVaultSecretModel.ExpiresOn.Value.Date + (ExpiresOnTimespan.HasValue ? ExpiresOnTimespan.Value : TimeSpan.Zero);

        var updatedProps = await _vaultService.UpdateSecret(KeyVaultSecretModel, KeyVaultSecretModel.VaultUri);
        KeyVaultSecretModel = updatedProps;
    }

    [RelayCommand]
    public async Task NewVersion()
    {
        var newSecret = new KeyVaultSecret(KeyVaultSecretModel.Name, SecretValue);
        if (KeyVaultSecretModel.NotBefore.HasValue)
            newSecret.Properties.NotBefore = KeyVaultSecretModel.NotBefore.Value.Date + (NotBeforeTimespan.HasValue ? NotBeforeTimespan.Value : TimeSpan.Zero);

        if (KeyVaultSecretModel.ExpiresOn.HasValue)
            newSecret.Properties.ExpiresOn = KeyVaultSecretModel.ExpiresOn.Value.Date + (ExpiresOnTimespan.HasValue ? ExpiresOnTimespan.Value : TimeSpan.Zero);

        newSecret.Properties.ContentType = KeyVaultSecretModel.ContentType;

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