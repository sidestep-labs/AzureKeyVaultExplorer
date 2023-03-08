using Azure.ResourceManager.KeyVault;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using sidestep.quickey.Services;
using System.Collections.ObjectModel;
using System.Linq;

namespace sidestep.quickey.ViewModel;

public partial class MainPageViewModel : ObservableObject
{
    private readonly AuthService _auth;
    private readonly VaultService _vaultSerivce;

    public MainPageViewModel(AuthService auth, VaultService vaultService)
    {
        _auth = auth;
        _vaultSerivce = vaultService;
        Username = Preferences.Get("username", null);
        Category = "Secrets";
        IsBusy = false;
        VaultList = new ObservableCollection<KeyVaultData>();
    }

    List<KeyVaultData> _vaultList  = new List<KeyVaultData>() { };

    [ObservableProperty]
    private ObservableCollection<KeyVaultData> vaultList;

    [ObservableProperty]
    private KeyVaultData selectedVault;

    //[ObservableProperty]
    //private ObservableCollection<KeyVaultData> secretList;

    //[ObservableProperty]
    //private ObservableCollection<KeyVaultData> keyList;

    //[ObservableProperty]
    //private ObservableCollection<KeyVaultData> certificateList;

    [ObservableProperty]
    private string category;

    [ObservableProperty]
    private string username;

    [ObservableProperty]
    private string vaultListFilter;

    private bool InitialLoad { get; set; } = true;

    [ObservableProperty]
    private bool isBusy;

    [RelayCommand]
    private void ToggleFlyout()
    {
        Shell.Current.FlyoutIsPresented = true;
    }

    [RelayCommand]
    private async void Appearing()
    {
        try
        {
            if (InitialLoad)
            {
                IsBusy = true;
                var keyVaultResources = _vaultSerivce.GetKeyVaultResources();
                await foreach (var kv in keyVaultResources)
                {
                    VaultList.Add(kv.Data);
                }
                InitialLoad = false;
                _vaultList.AddRange(VaultList);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.ToString());
        }
        finally
        {
            IsBusy = false;

        }
    }

    [RelayCommand]
    private async void Logout()
    {
        await _auth.RemoveAccount();
    }

    [RelayCommand]
    private async void FilterVaultList()
    {
        string query = VaultListFilter.Trim().ToLowerInvariant();
        //if (!string.IsNullOrWhiteSpace(query))
        var list = _vaultList.Where(v => v.Name.ToLowerInvariant().Contains(query));
        VaultList = new ObservableCollection<KeyVaultData>(list);
    }
}