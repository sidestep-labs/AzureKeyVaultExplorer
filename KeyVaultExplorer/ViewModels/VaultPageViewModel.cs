using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Notifications;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Layout;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Secrets;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentAvalonia.UI.Windowing;
using KeyVaultExplorer.Exceptions;
using KeyVaultExplorer.Models;
using KeyVaultExplorer.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace KeyVaultExplorer.ViewModels;

public partial class VaultPageViewModel : ViewModelBase
{
    [ObservableProperty]
    public bool isBusy = false;

    [ObservableProperty]
    public bool hasAuthorizationError = false;

    [ObservableProperty]
    public string authorizationMessage;

    [ObservableProperty]
    public string searchQuery;

    [ObservableProperty]
    public KeyVaultContentsAmalgamation selectedRow;

    [ObservableProperty]
    public TabStripItem selectedTab;

    [ObservableProperty]
    public ObservableCollection<KeyVaultContentsAmalgamation> vaultContents;

    [ObservableProperty]
    public Uri vaultUri;

    private readonly AuthService _authService;
    private readonly VaultService _vaultService;
    private SettingsPageViewModel _settingsPageViewModel;
    private NotificationViewModel _notificationViewModel;
    private Bitmap BitmapImage;
    private readonly IClipboard _clipboardService;

    public VaultPageViewModel()
    {
        _vaultService = Defaults.Locator.GetRequiredService<VaultService>();
        _authService = Defaults.Locator.GetRequiredService<AuthService>();
        _settingsPageViewModel = Defaults.Locator.GetRequiredService<SettingsPageViewModel>();
        _notificationViewModel = Defaults.Locator.GetRequiredService<NotificationViewModel>();
        _clipboardService = Defaults.Locator.GetRequiredService<IClipboard>();
        vaultContents = [];
        BitmapImage = new Bitmap(AssetLoader.Open(new Uri("avares://KeyVaultExplorer/Assets/kv-orange.ico"))).CreateScaledBitmap(new Avalonia.PixelSize(24, 24), BitmapInterpolationMode.HighQuality);

#if DEBUG
        for (int i = 0; i < 5; i++)
        {
            var sp = (new SecretProperties($"{i}_Demo__Key_Token") { ContentType = "application/json", Enabled = true, ExpiresOn = new System.DateTime(), });
            var item = new KeyVaultContentsAmalgamation
            {
                CreatedOn = new System.DateTime(),
                UpdatedOn = new System.DateTime(),
                Version = "version 1",
                VaultUri = new Uri("https://stackoverflow.com/"),
                ContentType = "application/json",
                Id = new Uri("https://stackoverflow.com/"),
                SecretProperties = sp
            };

            switch (i % 3)
            {
                case 0:
                    item.Name = $"{i}_Secret";
                    item.Type = KeyVaultItemType.Secret;
                    break;

                case 1:

                    item.Name = $"{i}__Key";
                    item.Type = KeyVaultItemType.Key;
                    break;

                case 2:
                    item.Name = $"{i}_Certificate";
                    item.Type = KeyVaultItemType.Key;
                    break;
            }
            VaultContents.Add(item);
        }
        _vaultContents = VaultContents;
#endif
    }

    public Dictionary<KeyVaultItemType, bool> LoadedItemTypes { get; set; } = new() { };
    private IEnumerable<KeyVaultContentsAmalgamation> _vaultContents { get; set; }

    public async Task ClearClipboardAsync()
    {
        await Task.Delay(_settingsPageViewModel.ClearClipboardTimeout * 1000); // convert to seconds
        await _clipboardService.ClearAsync();
    }

    public async Task FilterAndLoadVaultValueType(KeyVaultItemType item)
    {
        if (!LoadedItemTypes.ContainsKey(item))
        {
            IsBusy = true;
            switch (item)
            {
                case KeyVaultItemType.Certificate:
                    await GetCertificatesForVault(VaultUri);
                    LoadedItemTypes.TryAdd(item, true);
                    break;

                case KeyVaultItemType.Key:
                    await GetKeysForVault(VaultUri);
                    LoadedItemTypes.TryAdd(item, true);
                    break;

                case KeyVaultItemType.Secret:
                    await GetSecretsForVault(VaultUri);
                    LoadedItemTypes.TryAdd(item, true);
                    break;

                case KeyVaultItemType.All:
                    VaultContents.Clear();
                    await Task.WhenAny(GetSecretsForVault(VaultUri), GetKeysForVault(VaultUri), GetCertificatesForVault(VaultUri));
                    LoadedItemTypes.TryAdd(KeyVaultItemType.Secret, true);
                    LoadedItemTypes.TryAdd(KeyVaultItemType.Key, true);
                    LoadedItemTypes.TryAdd(KeyVaultItemType.Certificate, true);
                    LoadedItemTypes.TryAdd(KeyVaultItemType.All, true);
                    break;

                default:
                    break;
            }
        }
        if (item == KeyVaultItemType.All)
            VaultContents = new ObservableCollection<KeyVaultContentsAmalgamation>(_vaultContents.Where(v => v.Name.Contains(SearchQuery ?? "", StringComparison.OrdinalIgnoreCase)));
        else
            VaultContents = new ObservableCollection<KeyVaultContentsAmalgamation>(_vaultContents.Where(v => item == v.Type && v.Name.Contains(SearchQuery ?? "", StringComparison.OrdinalIgnoreCase)));

        await DelaySetIsBusy(false);
    }

    public async Task GetCertificatesForVault(Uri kvUri)
    {
        var certs = _vaultService.GetVaultAssociatedCertificates(kvUri);
        try
        {
            await foreach (var val in certs)
            {
                VaultContents.Add(new KeyVaultContentsAmalgamation
                {
                    Name = val.Name,
                    Id = val.Id,
                    Type = KeyVaultItemType.Certificate,
                    VaultUri = val.VaultUri,
                    ValueUri = val.Id,
                    Version = val.Version,
                    CertificateProperties = val,
                    Tags = val.Tags,
                    UpdatedOn = val.UpdatedOn,
                    CreatedOn = val.CreatedOn,
                    ExpiresOn = val.ExpiresOn,
                    Enabled = val.Enabled,
                    NotBefore = val.NotBefore,
                    RecoverableDays = val.RecoverableDays,
                    RecoveryLevel = val.RecoveryLevel
                });
            }
        }
        catch (Exception ex) when (ex.Message.Contains("403"))
        {
            HasAuthorizationError = true;
            AuthorizationMessage = ex.Message;
            Debug.WriteLine(ex.Message);
        }
        _vaultContents = VaultContents;
    }

    public async Task GetKeysForVault(Uri kvUri)
    {
        var keys = _vaultService.GetVaultAssociatedKeys(kvUri);
        try
        {
            await foreach (var val in keys)
            {
                VaultContents.Add(new KeyVaultContentsAmalgamation
                {
                    Name = val.Name,
                    Id = val.Id,
                    Type = KeyVaultItemType.Key,
                    VaultUri = val.VaultUri,
                    ValueUri = val.Id,
                    Version = val.Version,
                    KeyProperties = val,
                    Tags = val.Tags,
                    UpdatedOn = val.UpdatedOn,
                    CreatedOn = val.CreatedOn,
                    ExpiresOn = val.ExpiresOn,
                    Enabled = val.Enabled,
                    NotBefore = val.NotBefore,
                    RecoverableDays = val.RecoverableDays,
                    RecoveryLevel = val.RecoveryLevel
                });
            }
        }
        catch (Exception ex) when (ex.Message.Contains("403"))
        {
            HasAuthorizationError = true;
            AuthorizationMessage = ex.Message;
            Debug.WriteLine(ex.Message);
        }
        _vaultContents = VaultContents;
    }

    public async Task GetSecretsForVault(Uri kvUri)
    {
        var values = _vaultService.GetVaultAssociatedSecrets(kvUri);
        try
        {
            await foreach (var val in values)
            {
                VaultContents.Add(new KeyVaultContentsAmalgamation
                {
                    Name = val.Name,
                    Id = val.Id,
                    Type = KeyVaultItemType.Secret,
                    ContentType = val.ContentType,
                    VaultUri = val.VaultUri,
                    ValueUri = val.Id,
                    Version = val.Version,
                    SecretProperties = val,
                    Tags = val.Tags,
                    UpdatedOn = val.UpdatedOn,
                    CreatedOn = val.CreatedOn,
                    ExpiresOn = val.ExpiresOn,
                    Enabled = val.Enabled,
                    NotBefore = val.NotBefore,
                    RecoverableDays = val.RecoverableDays,
                    RecoveryLevel = val.RecoveryLevel
                });
            }
        }
        catch (Exception ex) when (ex.Message.Contains("403"))
        {
            HasAuthorizationError = true;
            AuthorizationMessage = ex.Message;
            Debug.WriteLine(ex.Message);
        }
        _vaultContents = VaultContents;
    }

    [RelayCommand]
    private async Task Copy(KeyVaultContentsAmalgamation keyVaultItem)
    {
        if (keyVaultItem is null) return;

        try
        {
            string value = string.Empty;
            //_ = keyVaultItem.Type switch
            //{
            //    KeyVaultItemType.Key => value = (await _vaultService.GetKey(keyVaultItem.VaultUri, keyVaultItem.Name)).Key.ToRSA().ToXmlString(true),
            //    KeyVaultItemType.Secret => value = (await _vaultService.GetSecret(keyVaultItem.VaultUri, keyVaultItem.Name)).Value,
            //    KeyVaultItemType.Certificate => value = (await _vaultService.GetCertificate(keyVaultItem.VaultUri, keyVaultItem.Name)).Name,
            //    _ => throw new NotImplementedException()
            //};

            if (keyVaultItem.Type == KeyVaultItemType.Key)
            {
                var key = await _vaultService.GetKey(keyVaultItem.VaultUri, keyVaultItem.Name);
                if (key.KeyType == KeyType.Rsa)
                {
                    using var rsa = key.Key.ToRSA();
                    var publicKey = rsa.ExportRSAPublicKey();
                    string pem = "-----BEGIN PUBLIC KEY-----\n" + Convert.ToBase64String(publicKey) + "\n-----END PUBLIC KEY-----";
                    value = pem;
                }
            }

            if (keyVaultItem.Type == KeyVaultItemType.Secret)
            {
                var sv = await _vaultService.GetSecret(keyVaultItem.VaultUri, keyVaultItem.Name);
                value = sv.Value;
            }
            if (keyVaultItem.Type == KeyVaultItemType.Certificate)
            {
                var certValue = await _vaultService.GetCertificate(keyVaultItem.VaultUri, keyVaultItem.Name);
            }

            // TODO: figure out why set data object async fails here.
            var dataObject = new DataObject();
            dataObject.Set(DataFormats.Text, value);
            await _clipboardService.SetTextAsync(value);
            ShowCopiedStatusNotification("Copied", $"The value of '{keyVaultItem.Name}' has been copied to the clipboard.", NotificationType.Success);
            _ = ClearClipboardAsync().ConfigureAwait(false);
        }
        catch (KeyVaultItemNotFoundException ex)
        {
            ShowCopiedStatusNotification($"A value was not found for '{keyVaultItem.Name}'", $"The value of was not able to be retrieved.\n {ex.Message}", NotificationType.Error);
        }
    }

    [RelayCommand]
    private async Task CopyUri(KeyVaultContentsAmalgamation keyVaultItem)
    {
        if (keyVaultItem is null) return;
        await _clipboardService.SetTextAsync(keyVaultItem.Id.ToString());
    }

    private async Task DelaySetIsBusy(bool val)
    {
        await Task.Delay(1000);
        IsBusy = val;
    }

    partial void OnSearchQueryChanged(string value)
    {
        var isValidEnum = Enum.TryParse(SelectedTab?.Name.ToString(), true, out KeyVaultItemType parsedEnumValue) && Enum.IsDefined(typeof(KeyVaultItemType), parsedEnumValue);
        var item = isValidEnum ? parsedEnumValue : KeyVaultItemType.Secret;
        string? query = value?.Trim();
        if (string.IsNullOrWhiteSpace(query))
        {
            var contents = _vaultContents;
            if (item != KeyVaultItemType.All)
            {
                contents = contents.Where(k => k.Type == item);
            }
            VaultContents = new ObservableCollection<KeyVaultContentsAmalgamation>(contents);
            return;
        }
        //var toFilter = CheckedBoxes.Where(v => v.Value == true).Select(s => s.Key).ToList();
        //       && toFilter.Contains(v.Type)

        var list = _vaultContents.Where(v => v.Name.Contains(query, StringComparison.OrdinalIgnoreCase)
                || (v.Tags is not null
                && v.Tags.Any(x => x.Value.Contains(query, StringComparison.OrdinalIgnoreCase)
                || x.Key.Contains(query, StringComparison.OrdinalIgnoreCase))
              ));

        if (item != KeyVaultItemType.All)
            list = list.Where(k => k.Type == item);
        VaultContents = new ObservableCollection<KeyVaultContentsAmalgamation>(list);
    }

    [RelayCommand]
    private void OpenInAzure(KeyVaultContentsAmalgamation keyVaultItem)
    {
        if (keyVaultItem is null) return;
        var uri = $"https://portal.azure.com/#@{_authService.TenantName}/asset/Microsoft_Azure_KeyVault/{keyVaultItem.Type}/{keyVaultItem.Id}";
        Process.Start(new ProcessStartInfo(uri) { UseShellExecute = true, Verb = "open" });
    }

    [RelayCommand]
    private async Task Refresh()
    {
        var isValidEnum = Enum.TryParse(SelectedTab?.Name, true, out KeyVaultItemType parsedEnumValue) && Enum.IsDefined(typeof(KeyVaultItemType), parsedEnumValue);
        var item = isValidEnum ? parsedEnumValue : KeyVaultItemType.Secret;
        LoadedItemTypes.Remove(item);
        VaultContents = new ObservableCollection<KeyVaultContentsAmalgamation>(_vaultContents.Where(v => v.Type != item));
        await FilterAndLoadVaultValueType(item);
    }

    private void ShowCopiedStatusNotification(string subject, string message, NotificationType notificationType)
    {
        //TODO: https://github.com/pr8x/DesktopNotifications/issues/26
        var notif = new Notification(subject, message, notificationType);
        _notificationViewModel.AddMessage(notif);
    }

    [RelayCommand]
    private void ShowProperties(KeyVaultContentsAmalgamation model)
    {
        if (model == null) return;

        var page = new PropertiesPage
        {
            DataContext = new PropertiesPageViewModel(model)
        };
        var taskDialog = new AppWindow
        {
            Title = $"{model.Type} {model.Name} Properties",
            Icon = BitmapImage,
            SizeToContent = SizeToContent.Manual,
            WindowStartupLocation = WindowStartupLocation.Manual,
            ShowAsDialog = false,
            CanResize = true,
            Content = page,
            Width = 620,
            Height = 480,
            MinWidth = 300,
            ExtendClientAreaToDecorationsHint = true,
            // TransparencyLevelHint = new List<WindowTransparencyLevel>() { WindowTransparencyLevel.Mica, WindowTransparencyLevel.AcrylicBlur },
            // Background = null,
        };

        var topLevel = Avalonia.Application.Current.GetTopLevel() as AppWindow;
        taskDialog.ShowDialog(topLevel);
    }
}