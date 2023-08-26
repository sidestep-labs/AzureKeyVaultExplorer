using Azure.Security.KeyVault.Secrets;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using kvexplorer.shared;
using System.Collections.ObjectModel;
using System.Threading;

namespace avalonia.kvexplorer.ViewModels;

public partial class VaultPageViewModel : ViewModelBase
{
    private readonly AuthService _authService;

    [ObservableProperty]
    public ObservableCollection<SecretProperties> secretList;

    public VaultPageViewModel(AuthService authService, VaultService vaultService)
    {
        _authService = authService;
    }

    public VaultPageViewModel()
    {
        _authService = new AuthService();
        secretList = new ObservableCollection<SecretProperties>()   {
            new SecretProperties("Salesforce Password" ) { ContentType = "application/json", Enabled = true, ExpiresOn = new System.DateTime(),  },
            new SecretProperties("SysAdminPassword" ) { ContentType = "application/json", Enabled = true, ExpiresOn = new System.DateTime(),  },
            new SecretProperties("AzureAPIKey" ) { ContentType = "application/json", Enabled = true, ExpiresOn = new System.DateTime(),  },
            new SecretProperties("AmazonAlexAuthToken" ) { ContentType = "application/json", Enabled = true, ExpiresOn = new System.DateTime(),  },
        };
    }
}