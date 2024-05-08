using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using Azure.Security.KeyVault.Certificates;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Secrets;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KeyVaultExplorer.Exceptions;
using KeyVaultExplorer.Models;
using KeyVaultExplorer.Services;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace KeyVaultExplorer.ViewModels;

public partial class PropertiesPageViewModel : ViewModelBase
{
    private readonly VaultService _vaultService;
    private readonly AuthService _authService;
    private readonly IClipboard _clipboardService;
    private readonly IStorageProvider _storageService;

    [ObservableProperty]
    public bool isSecret = false;

    [ObservableProperty]
    public bool isKey = false;

    [ObservableProperty]
    public bool isCertificate = false;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ShouldShowValueCommand))]
    public bool showValue = false;

    [ObservableProperty]
    public string secretHidden = new('*', 20);

    [ObservableProperty]
    public string secretPlainText = "";

    [ObservableProperty]
    public string title = "Properties";

    public PropertiesPageViewModel()
    {
        _vaultService = Defaults.Locator.GetRequiredService<VaultService>();
        _authService = Defaults.Locator.GetRequiredService<AuthService>();
        _clipboardService = Defaults.Locator.GetRequiredService<IClipboard>();
        _storageService = Defaults.Locator.GetRequiredService<IStorageProvider>();
    }

    [ObservableProperty]
    public ObservableCollection<KeyProperties> keyPropertiesList;

    [ObservableProperty]
    public ObservableCollection<SecretProperties> secretPropertiesList;

    [ObservableProperty]
    public ObservableCollection<CertificateProperties> certificatePropertiesList;

    [ObservableProperty]
    public KeyVaultContentsAmalgamation openedItem;

    private SettingsPageViewModel _settingsPageViewModel;

    public PropertiesPageViewModel(KeyVaultContentsAmalgamation model)
    {
        _vaultService = Defaults.Locator.GetRequiredService<VaultService>();
        _authService = Defaults.Locator.GetRequiredService<AuthService>();
        _settingsPageViewModel = Defaults.Locator.GetRequiredService<SettingsPageViewModel>();
        _clipboardService = Defaults.Locator.GetRequiredService<IClipboard>();
        _storageService = Defaults.Locator.GetRequiredService<IStorageProvider>();

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
                var certificateProperties = await _vaultService.GetCertificateProperties(model.VaultUri, model.Name);
                CertificatePropertiesList = new ObservableCollection<CertificateProperties>(certificateProperties);
                IsCertificate = true;
                break;

            case KeyVaultItemType.Key:
                var keyPropertiesList = new ObservableCollection<KeyProperties>(await _vaultService.GetKeyProperties(model.VaultUri, model.Name));

                KeyPropertiesList = new ObservableCollection<KeyProperties>(keyPropertiesList);
                IsKey = true;
                break;

            case KeyVaultItemType.Secret:
                var secretPropertiesList = new ObservableCollection<SecretProperties>(await _vaultService.GetSecretProperties(model.VaultUri, model.Name));
                SecretPropertiesList = new ObservableCollection<SecretProperties>(secretPropertiesList);
                IsSecret = true;
                break;

            default:
                IsSecret = false;
                IsCertificate = false;
                IsKey = false;
                break;
        }
        Title = $"{model.Type} {model.Name} Properties";
    }

    [RelayCommand]
    private async Task ShouldShowValue(bool val)
    {
        if (IsSecret && val)
        {
            var s = await _vaultService.GetSecret(kvUri: OpenedItem.SecretProperties.VaultUri, secretName: OpenedItem.SecretProperties.Name).ConfigureAwait(false);
            SecretPlainText = s.Value;
        }
    }

    [RelayCommand]
    private async Task Copy()
    {
        if (OpenedItem is null || IsCertificate) return;
        try
        {
            string value = string.Empty;
            if (IsKey)
            {
                var key = await _vaultService.GetKey(OpenedItem.KeyProperties.VaultUri, OpenedItem.KeyProperties.Name);
                if (key.KeyType == KeyType.Rsa)
                {
                    using var rsa = key.Key.ToRSA();
                    var publicKey = rsa.ExportRSAPublicKey();
                    string pem = "-----BEGIN PUBLIC KEY-----\n" + Convert.ToBase64String(publicKey) + "\n-----END PUBLIC KEY-----";
                    value = pem;
                }
            }

            if (IsSecret)
            {
                var sv = await _vaultService.GetSecret(OpenedItem.SecretProperties.VaultUri, OpenedItem.SecretProperties.Name);
                value = sv.Value;
            }

            // TODO: figure out why set data object async fails here.
            var dataObject = new DataObject();
            dataObject.Set(DataFormats.Text, value);
            await _clipboardService.SetTextAsync(value);
            ClearClipboardAsync().ConfigureAwait(false);
        }
        catch (KeyVaultItemNotFoundException ex)
        {
        }
    }

    public async Task ClearClipboardAsync()
    {
        await Task.Delay(_settingsPageViewModel.ClearClipboardTimeout * 1000); // convert to seconds
        await _clipboardService.ClearAsync();
    }

    [RelayCommand]
    private async Task Download(string exportType)
    {
        if (exportType == "Key")
        {
            var key = await _vaultService.GetKey(OpenedItem.KeyProperties.VaultUri, OpenedItem.KeyProperties.Name);
            using var rsa = key.Key.ToRSA();
            var publicKey = rsa.ExportRSAPublicKey();
            string pem = "-----BEGIN PUBLIC KEY-----\n" + Convert.ToBase64String(publicKey) + "\n-----END PUBLIC KEY-----";
            SaveFile(OpenedItem.KeyProperties.Name, content: pem, ext: "pem");
        }
        else
        {
            var certificateWithPolicy = await _vaultService.GetCertificate(OpenedItem.CertificateProperties.VaultUri, OpenedItem.CertificateProperties.Name);
            // Create X.509 certificate from bytes
            var certificate = new X509Certificate2(certificateWithPolicy.Cer);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("-----BEGIN CERTIFICATE-----");
            var ext = "cer";
            if (exportType == nameof(X509ContentType.Cert))
            {
                sb.AppendLine(Convert.ToBase64String(certificate.Export(X509ContentType.Cert), Base64FormattingOptions.None));
            }
            else if (exportType == nameof(X509ContentType.Pfx))
            {
                ext = "pfx";
                sb.AppendLine(Convert.ToBase64String(certificate.Export(X509ContentType.Pfx), Base64FormattingOptions.None));
            }
            sb.AppendLine("-----END CERTIFICATE-----");
            SaveFile(OpenedItem.CertificateProperties.Name, content: sb.ToString(), ext: ext);
        }
    }

    private async void SaveFile(string fileName, string ext, string content)
    {
        var desktopFolder = await _storageService.TryGetWellKnownFolderAsync(WellKnownFolder.Downloads);
        var file = await _storageService.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = $"Save {fileName}",
            SuggestedFileName = fileName,
            SuggestedStartLocation = desktopFolder,
            DefaultExtension = ext
        });

        if (file is not null)
        {
            await using var stream = await file.OpenWriteAsync();

            using var streamWriter = new StreamWriter(stream);
            await streamWriter.WriteLineAsync(content);
        }
    }

    [RelayCommand]
    private void OpenInAzure()
    {
        if (OpenedItem is null) return;
        var uri = $"https://portal.azure.com/#@{_authService.TenantName}/asset/Microsoft_Azure_KeyVault/{OpenedItem.Type}/{OpenedItem.Id}";
        Process.Start(new ProcessStartInfo(uri) { UseShellExecute = true, Verb = "open" });
    }
}