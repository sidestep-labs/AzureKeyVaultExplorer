using avalonia.kvexplorer.Views;
using avalonia.kvexplorer.Views.Pages;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Notifications;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Azure.ResourceManager.KeyVault;
using Azure.Security.KeyVault.Secrets;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentAvalonia.UI.Controls;
using kvexplorer.shared;
using kvexplorer.shared.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace avalonia.kvexplorer.ViewModels;

public partial class VaultPageViewModel : ViewModelBase
{
    [ObservableProperty]
    public bool isBusy = true;

    [ObservableProperty]
    public bool isCertificatesChecked = true;

    [ObservableProperty]
    public bool isKeysChecked = true;

    [ObservableProperty]
    public bool isSecretsChecked = true;

    [ObservableProperty]
    public string searchQuery;

    [ObservableProperty]
    public ObservableCollection<KeyVaultContentsAmalgamation> vaultContents;

    private readonly VaultService _vaultService;

    public VaultPageViewModel()
    {
        _vaultService = Defaults.Locator.GetRequiredService<VaultService>();

        vaultContents = new ObservableCollection<KeyVaultContentsAmalgamation>() { };

        for (int i = 0; i < 50; i++)
        {
            var sp = (new SecretProperties($"{i}_Demo__Key_Token") { ContentType = "application/json", Enabled = true, ExpiresOn = new System.DateTime(), });

            switch (i % 3)
            {
                case 0:
                    VaultContents.Add(new KeyVaultContentsAmalgamation
                    {
                        Name = $"{i}_Secret",
                        Id = new Uri("https://stackoverflow.com/"),
                        Type = KeyVaultItemType.Secret,
                        ContentType = "application/json",
                        VaultUri = new Uri("https://stackoverflow.com/"),
                        Version = "version 1",
                        SecretProperties = sp,
                    });
                    break;

                case 1:
                    VaultContents.Add(new KeyVaultContentsAmalgamation
                    {
                        Name = $"{i}__Key",
                        Id = new Uri("https://stackoverflow.com/"),
                        Type = KeyVaultItemType.Key,
                        ContentType = "application/json",
                        VaultUri = new Uri("https://stackoverflow.com/"),
                        Version = "version 1",
                        SecretProperties = sp,
                    });
                    break;

                case 2:
                    VaultContents.Add(new KeyVaultContentsAmalgamation
                    {
                        Name = $"{i}_Certificate",
                        Id = new Uri("https://stackoverflow.com/"),
                        Type = KeyVaultItemType.Certificate,
                        ContentType = "application/json",
                        VaultUri = new Uri("https://stackoverflow.com/"),
                        Version = "version 1",
                        SecretProperties = sp,
                    });
                    break;
            }
            _vaultContents = VaultContents;
        }
    }

    public Dictionary<KeyVaultItemType, bool> CheckedBoxes { get; set; } = new Dictionary<KeyVaultItemType, bool>() {
          { KeyVaultItemType.Key, true},
          { KeyVaultItemType.Secret, true},
          { KeyVaultItemType.Certificate, true},
    };
    private IEnumerable<KeyVaultContentsAmalgamation> _vaultContents { get; set; }

    /*
     *
      get => new DataGridCollectionView(VaultContents)
        {
            GroupDescriptions = { new DataGridPathGroupDescription("Name") }
        }
     */
    //    public async Task<IEnumerable<KeyVaultContentsAmalgamation>> GetSecretsForVault(Uri kvUri)

    public void FilterBasedOnCheckedBoxes()
    {
        var toFilter = CheckedBoxes.Where(v => v.Value == true).Select(s => s.Key).ToList();
        VaultContents = new ObservableCollection<KeyVaultContentsAmalgamation>(_vaultContents.Where(v => toFilter.Contains(v.Type) && v.Name.ToLowerInvariant().Contains(SearchQuery ?? "")));
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
        _vaultContents = VaultContents;

        //return _vaultContents;
    }

    partial void OnIsCertificatesCheckedChanged(bool value)
    {
        CheckedBoxes[KeyVaultItemType.Certificate] = value;
    }

     partial void OnIsKeysCheckedChanged(bool value)
    {
        CheckedBoxes[KeyVaultItemType.Key] = value;

        //Dispatcher.UIThread.Post(() => FilterBasedOnCheckedBoxes(), DispatcherPriority.Input);
    }

     partial void OnIsSecretsCheckedChanged(bool value)
    {
        CheckedBoxes[KeyVaultItemType.Secret] = value;
    }

    partial void OnSearchQueryChanged(string value)
    {
        string query = value.Trim().ToLowerInvariant();
        if (!string.IsNullOrWhiteSpace(query))
        {
            VaultContents = new ObservableCollection<KeyVaultContentsAmalgamation>(_vaultContents);
        }
        var toFilter = CheckedBoxes.Where(v => v.Value == true).Select(s => s.Key).ToList();
        var list = _vaultContents.Where(v => v.Name.ToLowerInvariant().Contains(query) && toFilter.Contains(v.Type));
        VaultContents = new ObservableCollection<KeyVaultContentsAmalgamation>(list);
    }

    [RelayCommand]
    private async Task Copy(KeyVaultContentsAmalgamation keyVaultItem)
    {
        var secret = await _vaultService.GetSecret( keyVaultItem.VaultUri,keyVaultItem.Name);
        var topLevel = (Avalonia.Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime).MainWindow;
        var clipboard = TopLevel.GetTopLevel(topLevel)?.Clipboard;
        var dataObject = new DataObject();
        dataObject.Set(DataFormats.Text, secret.Value);
        await clipboard.SetDataObjectAsync(dataObject);


        var not = new Notification("Copied", $"The value of {keyVaultItem.Name} has been copied to the clipboard.", NotificationType.Success);
        var nm = new WindowNotificationManager(topLevel)
        {
            Position = NotificationPosition.BottomRight,
            MaxItems = 1,
        };
        nm.TemplateApplied += (sender, args) =>
        {
            nm.Show(not);
        };
    }

}
