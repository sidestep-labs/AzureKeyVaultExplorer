using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using sidestep.quickey.Services;

namespace sidestep.quickey.ViewModel;

public partial class MainPageViewModel : ObservableObject
{
    private readonly AuthService _auth;
    public MainPageViewModel(AuthService auth)
    {
        _auth = auth;
        username = Preferences.Get("username", null);
        category = "Secrets";
    }


    [ObservableProperty]
    private string category;


    [ObservableProperty]
    private string username;


    [RelayCommand]
     void ToggleFlyout()
    {
        Shell.Current.FlyoutIsPresented = true;
    }


    [RelayCommand]
    async void Logout()
    {
       await _auth.RemoveAccount();
    }

}