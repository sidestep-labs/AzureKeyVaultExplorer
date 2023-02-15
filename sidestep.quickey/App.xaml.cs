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
        window.Title= "KeyVault Explorer";
        const int newWidth = 900;
        const int newHeight = 650;

        window.Width = newWidth;
        window.Height = newHeight;

        return window;
    }

   
}