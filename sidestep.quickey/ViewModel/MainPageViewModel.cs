using Azure.ResourceManager.KeyVault;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using sidestep.quickey.Services;
using System.Collections.ObjectModel;

namespace sidestep.quickey.ViewModel;

public partial class MainPageViewModel : ObservableObject
{
    private readonly AuthService _auth;
    private readonly VaultService _vaultSerivce;

    public MainPageViewModel(AuthService auth, VaultService vaultService)
    {
        _auth = auth;
        _vaultSerivce = vaultService;
        username = Preferences.Get("username", null);
        category = "Secrets";
        isBusy = false;
        VaultList = new ObservableCollection<KeyVaultData>();
    }



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
}