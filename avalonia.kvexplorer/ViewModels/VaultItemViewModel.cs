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

public partial class VaultItemViewModel : ViewModelBase
{
    private readonly VaultService _vaultService;

    [ObservableProperty]
    private ObservableCollection<SecretProperties> secretList;

    [ObservableProperty]
    private string header;

    [ObservableProperty]
    public IconSource iconSource;

    public VaultItemViewModel()
    {
        _vaultService = Defaults.Locator.GetRequiredService<VaultService>();
        secretList = new()
        {
            new SecretProperties("Salesforce Password" ) { ContentType = "application/json", Enabled = true, ExpiresOn = new System.DateTime(),  },
        };
    }
}