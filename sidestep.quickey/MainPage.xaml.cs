namespace sidestep.quickey;

public partial class MainPage : ContentPage
{
    private int count = 0;

    public MainPage()
    {
        InitializeComponent();
    }

    private void Go(object sender, EventArgs e)
    {
        //if (Preferences.Get("is_authenticated", false) == false)
        Shell.Current.GoToAsync(nameof(AuthenticationPage));
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
}