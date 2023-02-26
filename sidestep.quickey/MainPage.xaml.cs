namespace sidestep.quickey;

public partial class MainPage : ContentPage
{
    private int count = 0;

    public MainPage()
    {
        InitializeComponent();
    }

    private async void Go(object sender, EventArgs e)
    {
        //if (Preferences.Get("is_authenticated", false) == false)
        await Shell.Current.GoToAsync(nameof(AuthenticationPage));
    }

    private void OnCounterClicked(object sender, EventArgs e)
    {
        //Preferences.Clear();
        count++;
        if (count == 1)
            CounterBtn.Text = $"Clicked {count} time";
        else
            CounterBtn.Text = $"Clicked {count} times";

        SemanticScreenReader.Announce(CounterBtn.Text);
    }

    private void Toggle_Thene(object sender, EventArgs e)
    {
        var currentTheme = Application.Current.UserAppTheme;
        // || Application.Current.RequestedTheme == AppTheme.Light
        if (currentTheme == AppTheme.Light || currentTheme == AppTheme.Unspecified)
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
        Window secondWindow = new Window(new MainPage());

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
}