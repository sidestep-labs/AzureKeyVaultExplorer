using avalonia.kvexplorer.Views.CustomControls;
using Azure.Security.KeyVault.Secrets;
using CommunityToolkit.Mvvm.ComponentModel;
using FluentAvalonia.UI.Controls;
using System;
using System.Collections.ObjectModel;

namespace avalonia.kvexplorer.ViewModels.Models;

public partial class DocumentItem : ObservableObject
{
    public string Header { get; set; }

    public IconSource IconSource { get; set; }

    public string Content { get; set; }

    public Vault Vault { get; set; }

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


public partial class SecretPropertiesX : SecretProperties
{
    public SecretPropertiesX(string name) : base(name)
    {
    }

    public SecretPropertiesX(Uri id) : base(id)
    {
    }
}