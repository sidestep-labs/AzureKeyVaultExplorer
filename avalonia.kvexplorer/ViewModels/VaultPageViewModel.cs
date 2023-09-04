using Azure.ResourceManager.KeyVault;
using Azure.Security.KeyVault.Secrets;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using kvexplorer.shared;
using kvexplorer.shared.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace avalonia.kvexplorer.ViewModels;

public partial class VaultPageViewModel : ViewModelBase
{
    private readonly VaultService _vaultService;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(FilterValuesCommand))]
    public string searchQuery;
   
    private IEnumerable<KeyVaultContentsAmalgamation> _vaultContents { get; set; }

    [ObservableProperty]
    public ObservableCollection<KeyVaultContentsAmalgamation> vaultContents;

    [ObservableProperty]
    public bool isKeysChecked = true;

    [ObservableProperty]
    public bool isSecretsChecked = true;

    [ObservableProperty]
    public bool isCertificatesChecked = true;

    public VaultPageViewModel()
    {
        _vaultService = Defaults.Locator.GetRequiredService<VaultService>();

        vaultContents = new ObservableCollection<KeyVaultContentsAmalgamation>() { };

        for (int i = 0; i < 100; i++)
        {
            var sp = (new SecretProperties($"{i}_Demo__Key_Token") { ContentType = "application/json", Enabled = true, ExpiresOn = new System.DateTime(), });
            VaultContents.Add(new KeyVaultContentsAmalgamation
            {
                Name = $"{i}_Demo__Key_Token",
                Id = new Uri("https://stackoverflow.com/"),
                Type = KeyVaultItemType.Secret,
                ContentType = "application/json",
                VaultUri = new Uri("https://stackoverflow.com/"),
                Version = "version 1",
                SecretProperties = sp,
            });
        }
        _vaultContents = VaultContents.ToArray();
    }

    public async Task GetSecretsForVault(Uri kvUri)
    {
        var values = _vaultService.GetVaultAssociatedSecrets(kvUri);
        await foreach (var secret in values)
        {
            VaultContents.Add(new KeyVaultContentsAmalgamation
            {
                Name = secret.Name,
                Id = secret.Id,
                Type = KeyVaultItemType.Secret,
                ContentType = secret.ContentType,
                VaultUri = secret.VaultUri,
                Version = secret.Version,
                SecretProperties = secret,
            });
        }
        _vaultContents = VaultContents ;
    }

    [RelayCommand]
    private void FilterValues()
    {
        string query = SearchQuery.Trim().ToLowerInvariant();
        //if (!string.IsNullOrWhiteSpace(query))
        var list = VaultContents.Where(v => v.Name.ToLowerInvariant().Contains(query));
        VaultContents = new ObservableCollection<KeyVaultContentsAmalgamation>(list);
    }

     partial void OnSearchQueryChanged(string value)
    {
        string query = value.Trim().ToLowerInvariant();
        if (!string.IsNullOrWhiteSpace(query))
        {
            VaultContents = new ObservableCollection<KeyVaultContentsAmalgamation>(_vaultContents);
        }
        var list = _vaultContents.Where(v => v.Name.ToLowerInvariant().Contains(query));
        VaultContents = new ObservableCollection<KeyVaultContentsAmalgamation>(list);
    }
}