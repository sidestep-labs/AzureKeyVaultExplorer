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
        window.Title = "KeyVault Explorer";
        window.Width = 900; window.Height = 650;
 

        return window;
    }
}