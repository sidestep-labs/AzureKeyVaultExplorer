using Azure.Security.KeyVault.Secrets;
using Azure.Security.KeyVault.Certificates;
using Azure.Security.KeyVault.Keys;
using CommunityToolkit.Mvvm.ComponentModel;
using kvexplorer.shared;
using kvexplorer.shared.Database;
using kvexplorer.shared.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Threading;

namespace avalonia.kvexplorer.ViewModels;

public partial class PropertiesPageViewModel : ViewModelBase
{
    private readonly VaultService _vaultService;

    public PropertiesPageViewModel()
    {
        _vaultService = Defaults.Locator.GetRequiredService<VaultService>();
    }

    [ObservableProperty]
    public ObservableCollection<KeyProperties> keyPropertiesList;

    [ObservableProperty]
    public ObservableCollection<SecretProperties> secretPropertiesList;

    [ObservableProperty]
    public ObservableCollection<CertificateProperties> certificatePropertiesList;

    [ObservableProperty]
    public KeyVaultContentsAmalgamation openedItem;

    public PropertiesPageViewModel(KeyVaultContentsAmalgamation model)
    {
        _vaultService = Defaults.Locator.GetRequiredService<VaultService>();
        OpenedItem = model;
        Dispatcher.UIThread.InvokeAsync(async () =>
        {
            await GetPropertiesForKeyVaultValue(model);
        }, priority: DispatcherPriority.Normal);
    }

    private async Task GetPropertiesForKeyVaultValue(KeyVaultContentsAmalgamation model)
    {
        switch (model.Type)
        {
            case KeyVaultItemType.Certificate:
                CertificatePropertiesList = new ObservableCollection<CertificateProperties>(await _vaultService.GetCertificateProperties(model.VaultUri, model.Name));
                break;

            case KeyVaultItemType.Key:
                KeyPropertiesList = new ObservableCollection<KeyProperties>(await _vaultService.GetKeyProperties(model.VaultUri, model.Name));
                break;

            case KeyVaultItemType.Secret:
                SecretPropertiesList = new ObservableCollection<SecretProperties>(await _vaultService.GetSecretProperties(model.VaultUri, model.Name));
                break;

            default:
                break;
        }
    }
}