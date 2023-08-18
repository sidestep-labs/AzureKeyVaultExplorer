using Azure.ResourceManager.KeyVault;
using Azure.Security.KeyVault.Secrets;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentAvalonia.UI.Controls;
using kvexplorer.shared;
using kvexplorer.shared.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace avalonia.kvexplorer.ViewModels;

public partial class KeyVaultPageViewModel : ViewModelBase
{
    [ObservableProperty]
    public string searchQuery;

    [ObservableProperty]
    public KeyVaultResource selectedTreeItem;

    [ObservableProperty]
    public ObservableCollection<KeyVaultModel> vaultTreeList;

    private readonly AuthService _authService;
    private readonly VaultService _vaultService;

    [ObservableProperty]
    private ObservableCollection<SecretProperties> secretList;

    public KeyVaultPageViewModel()
    {
        _authService = Defaults.Locator.GetRequiredService<AuthService>();
        _vaultService = Defaults.Locator.GetRequiredService<VaultService>();
        PropertyChanged += OnMyViewModelPropertyChanged;

        vaultTreeList = new ObservableCollection<KeyVaultModel>
        {
            new KeyVaultModel
            {
                SubscriptionDisplayName = "Sandbox Subscription",
                SubscriptionId = "123",
                KeyVaultResources = new List<KeyVaultResource>{ }
            },
        };

        secretList = new()
            {
                new SecretProperties("Salesforce Password") { ContentType = "application/json", Enabled = true, ExpiresOn = new System.DateTime(), },
        };

        //Dispatcher.UIThread.Post(() => GetAvailableKeyVaults(), DispatcherPriority.Default);
    }

    public TitleBarViewModel TitleBarViewModel { get; set; }

    [RelayCommand]
    public async Task GetAvailableKeyVaults()
    {
        await Login();
        var resource = _vaultService.GetKeyVaultResourceBySubscriptionAndResourceGroup();
        await foreach (var item in resource)
        {
            VaultTreeList.Add(item);
        }
    }

    public ObservableCollection<DocumentItem> Documents { get; }

    public FACommand AddDocumentCommand { get; }

    private void AddDocumentExecute(object obj)
    {
        Documents.Add(AddDocument(Documents.Count));
    }

    public class DocumentItem
    {
        public string Header { get; set; }

        public IconSource IconSource { get; set; }

        public string Content { get; set; }
    }

    private DocumentItem AddDocument(int index)
    {
        var tab = new DocumentItem
        {
            Header = $"My document {index}"
        };

        switch (index % 3)
        {
            case 0:
                tab.IconSource = new SymbolIconSource { Symbol = Symbol.Document };
                tab.Content = "This is a sample document. Switch tabs to view more.";
                break;

            case 1:
                tab.IconSource = new SymbolIconSource { Symbol = Symbol.Star };
                tab.Content = "This is another sample document. Switch tabs to view more.";
                break;

            case 2:
                tab.IconSource = new SymbolIconSource { Symbol = Symbol.Open };
                tab.Content = "This is yet another sample document. Switch tabs to view more.";
                break;
        }

        return tab;
    }

    private DocumentItem _keybindingSelectedDocument;

    [RelayCommand]
    private async Task Login()
    {
        var cancellation = new CancellationToken();
        var account = await _authService.RefreshTokenAsync(cancellation);
        if (account == null)
            await _authService.LoginAsync(cancellation);
    }

    //if (SelectedTreeViewItems == null) return;

    //var vault = _vaultService.GetVaultAssociatedSecrets(SelectedTreeViewItems);
    //await foreach (var secret in vault)
    //{
    //    SecretList.Add(secret);
    //}

    private void OnMyViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SelectedTreeItem))
        {
            // Handle changes to the SelectedTreeItem property here
            OnSelectedTreeItemChanged("test");
        }
    }

    private async void OnSelectedTreeItemChanged(object value)
    {
        // Handle the SelectedTreeItem property change event here
        if (SelectedTreeItem == null) return;
        var vault = _vaultService.GetVaultAssociatedSecrets(SelectedTreeItem.Data.Properties.VaultUri);
        await foreach (var secret in vault)
        {
            SecretList.Add(secret);
            Debug.WriteLine($"value, {value}");
        }
    }



}