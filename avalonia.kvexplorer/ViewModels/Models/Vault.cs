using Azure.Security.KeyVault.Secrets;
using CommunityToolkit.Mvvm.ComponentModel;
using FluentAvalonia.UI.Controls;
using kvexplorer.shared;
using System.Collections.ObjectModel;
using static kvexplorer.shared.VaultService;

namespace avalonia.kvexplorer.ViewModels.Models;

public partial class Vault : ObservableObject
{
    //private readonly VaultService _vaultService;

    [ObservableProperty]
    private ObservableCollection<SecretProperties> secretList;

    [ObservableProperty]
    private string header;

    [ObservableProperty]
    public IconSource iconSource;

    public Vault()
    {
       // _vaultService = Defaults.Locator.GetRequiredService<VaultService>();
        secretList = new()
        {
            new SecretProperties("Salesforce Password" ) { ContentType = "application/json", Enabled = true, ExpiresOn = new System.DateTime(),  },
        };
    }
}