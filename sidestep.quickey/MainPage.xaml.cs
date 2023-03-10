using CommunityToolkit.Maui.Alerts;
using sidestep.quickey.Services;
using sidestep.quickey.ViewModel;
using sidestep.quickey.Views;

namespace sidestep.quickey;

public partial class MainPage : ContentPage
{
    private int count = 0;
    MainPageViewModel _viewModel;
    public MainPage(MainPageViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        _viewModel = vm;
    }

    private async void Go(object sender, EventArgs e)
    {
        //if (Preferences.Get("is_authenticated", false) == false)
        await Shell.Current.GoToAsync(nameof(AuthenticationPage), animate: false);
    }


  


    //private void OnCounterClicked(object sender, EventArgs e)
    //{
    //    //Preferences.Clear();
    //    count++;
    //    if (count == 1)
    //        CounterBtn.Text = $"Clicked {count} time";
    //    else
    //        CounterBtn.Text = $"Clicked {count} times";

    //    SemanticScreenReader.Announce(CounterBtn.Text);
    //}

    private void Toggle_Thene(object sender, EventArgs e)
    {
        var currentTheme = Application.Current.UserAppTheme;
        if (currentTheme == AppTheme.Light || Application.Current.RequestedTheme == AppTheme.Light)
        {
            Application.Current.UserAppTheme = AppTheme.Dark;
            Preferences.Set("preferred_theme", "dark");
        }
        else
        {
            Application.Current.UserAppTheme = AppTheme.Light;
            Preferences.Set("preferred_theme", "light");
        }
    }

    private void newWindow_Clicked(object sender, EventArgs e)
    {
        Window secondWindow = new Window(new WelcomePage());

        //secondWindow.MinimumWidth = 700;
        //secondWindow.MaximumWidth = 700;
        secondWindow.Title = "new window";
        secondWindow.Width = 500;
        secondWindow.Height = 500;

#if MACCATALYST
        // dispatcher is used to give the window time to actually resize
        Dispatcher.Dispatch(() =>
        {
            Window.MinimumWidth = 0;
            Window.MaximumWidth = double.PositiveInfinity;
            Window.MinimumHeight = 0;
            Window.MaximumHeight = double.PositiveInfinity;
        });

#endif
        Application.Current.OpenWindow(secondWindow);
    }

    private void VaultListFilterEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        _viewModel.FilterVaultListCommand.Execute(null);
    }

    private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        _viewModel.VaultSelectedCommand.Execute(null);
    }
}