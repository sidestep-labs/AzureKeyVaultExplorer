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
        secondWindow.Width = 400;
        secondWindow.Height = 500;

        secondWindow.MaximumWidth = 400;
        secondWindow.MaximumHeight = 500;

        Dispatcher.Dispatch(() =>
        {
            Window.MinimumWidth = 400;
            Window.MaximumWidth = double.PositiveInfinity;
            Window.MinimumHeight = 400;
            Window.MaximumHeight = double.PositiveInfinity;
        });
        Application.Current.OpenWindow(secondWindow);
    }
}