using Azure.Security.KeyVault.Secrets;
using CommunityToolkit.Mvvm.ComponentModel;
using FluentAvalonia.UI.Controls;
using kvexplorer.shared.Models;
using System.Collections.ObjectModel;

namespace kvexplorer.ViewModels.Models;

public partial class DocumentItem : ObservableObject
{
    public string Header { get; set; }

    public IconSource IconSource { get; set; }

    public object Content { get; set; }

    [ObservableProperty]
    public ObservableCollection<KeyVaultContentsAmalgamation> vaultContents;

    [ObservableProperty]
    public ObservableCollection<SecretProperties> secretList;

    public DocumentItem()
    {
        secretList = new ObservableCollection<SecretProperties>()   {
            new SecretProperties("Salesforce Password" ) { ContentType = "application/json", Enabled = true, ExpiresOn = new System.DateTime(),  },
            new SecretProperties("SysAdminPassword" ) { ContentType = "application/json", Enabled = true, ExpiresOn = new System.DateTime(),  },
            new SecretProperties("AzureAPIKey" ) { ContentType = "application/json", Enabled = true, ExpiresOn = new System.DateTime(),  },
            new SecretProperties("AmazonAlexAuthToken" ) { ContentType = "application/json", Enabled = true, ExpiresOn = new System.DateTime(),  },
        };
    }
}