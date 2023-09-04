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

    [ObservableProperty]
    public ObservableCollection<SecretProperties> secretList;

    private IEnumerable<SecretProperties> _secretList { get; set; }

    [ObservableProperty]
    public ObservableCollection<KeyVaultContentsAmalgamation> vaultContents;

    [ObservableProperty]
    public ObservableCollection<SecretProperties> secretListFiltered;

    public VaultPageViewModel(string vaultIdentifier)
    {
        _vaultService = Defaults.Locator.GetRequiredService<VaultService>();
    }

    public VaultPageViewModel()
    {
        _vaultService = Defaults.Locator.GetRequiredService<VaultService>();

        secretList = new ObservableCollection<SecretProperties>() { };
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
            secretList.Add(sp);
        }
        _secretList = secretList.ToList();
    }

    public async Task GetSecretsForVault(Uri kvUri)
    {
        var values = _vaultService.GetVaultAssociatedSecrets(kvUri);
        await foreach (var secret in values)
        {
            SecretList.Add(secret);

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
        _secretList = SecretList.ToList();
    }

    [RelayCommand]
    private void FilterValues()
    {
        string query = SearchQuery.Trim().ToLowerInvariant();
        //if (!string.IsNullOrWhiteSpace(query))
        var list = SecretList.Where(v => v.Name.ToLowerInvariant().Contains(query));
        SecretList = new ObservableCollection<SecretProperties>(list);
    }

     partial void OnSearchQueryChanged(string value)
    {
        string query = value.Trim().ToLowerInvariant();
        if (!string.IsNullOrWhiteSpace(query))
        {
            SecretList = new ObservableCollection<SecretProperties>(_secretList);
        }
        var list = _secretList.Where(v => v.Name.ToLowerInvariant().Contains(query));
        SecretList = new ObservableCollection<SecretProperties>(list);
    }
}