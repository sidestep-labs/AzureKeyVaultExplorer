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

    private void newWindow_Clicked(object sender, EventArgs e)
    {
        Window secondWindow = new Window(new MainPage());
     
        secondWindow.MinimumWidth = 700;
        secondWindow.MaximumWidth = 700;
        Window.Title = "new window";
        secondWindow.MinimumHeight = 500;
        secondWindow.MaximumHeight = 500;

#if MACCATALYST
        // dispatcher is used to give the window time to actually resize
        Dispatcher.Dispatch(() =>
        {
            Window.MinimumWidth = 0;
            Window.MaximumWidth = double.PositiveInfinity;
            Window.MinimumHeight = 0;
            Window.MaximumHeight = double.PositiveInfinity;

            Window.Title = "new window";
            Window.Width = 200;
            Window.Height = 250;

        });

#endif
        Application.Current.OpenWindow(secondWindow);


    }
}