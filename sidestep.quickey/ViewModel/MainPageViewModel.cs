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
        VaultList = new ObservableCollection<KeyVaultData>();
    }

    private bool InitialLoad { get; set; } = true;

    [ObservableProperty]
    private ObservableCollection<KeyVaultData> vaultList;

    [ObservableProperty]
    private string category;

    [ObservableProperty]
    private string username;

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
    }

    [RelayCommand]
    private async void Logout()
    {
        await _auth.RemoveAccount();
    }
}