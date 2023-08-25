using Avalonia.Threading;
using Azure.ResourceManager.KeyVault;
using Azure.Security.KeyVault.Secrets;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentAvalonia.UI.Controls;
using kvexplorer.shared;
using kvexplorer.shared.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static kvexplorer.shared.VaultService;

namespace avalonia.kvexplorer.ViewModels;

public partial class TabViewPageViewModel : ViewModelBase
{


    private readonly VaultService _vaultService;

    [ObservableProperty]
    private ObservableCollection<SecretProperties> secretList;

    public TabViewPageViewModel()
    {
        _vaultService = Defaults.Locator.GetRequiredService<VaultService>();

       
        secretList = new()
        {
            new SecretProperties("Salesforce Password") { ContentType = "application/json", Enabled = true, ExpiresOn = new System.DateTime(), },
        };
     
    }

    

    #region later

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

    #endregion later

    
}