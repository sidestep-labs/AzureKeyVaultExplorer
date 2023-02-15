namespace sidestep.quickey;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new AppShell();
    }

    protected override Window CreateWindow(IActivationState activationState)
    {
        var window = base.CreateWindow(activationState);
        var name = Preferences.Get("username", "");
        window.Title = "KeyVault Explorer" + $"- {name}"; 
        window.Width = 900;
        window.Height = 650;

        return window;
    }
}