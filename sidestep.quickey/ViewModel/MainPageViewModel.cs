using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using sidestep.quickey.Services;

namespace sidestep.quickey.ViewModel;

public partial class MainPageViewModel : ObservableObject
{
    private readonly AuthService _auth;
    public MainPageViewModel(AuthService auth)
    {
        category = "Secrets";
        _auth = auth;
    }


    [ObservableProperty]
    private string category;


    [RelayCommand]
     void ToggleFlyout()
    {
        Shell.Current.FlyoutIsPresented = true;
    }


    [RelayCommand]
    async void Logout()
    {
       await _auth.Logout();
    }

}