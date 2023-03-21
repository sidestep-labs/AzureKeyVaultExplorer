using Microsoft.Extensions.Configuration;

namespace kvexplorer;

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
        var name = Preferences.Get("username", null) ?? Preferences.Get("name", null);
        window.Title = "KeyVault Explorer" + (name != null ? $" — {name}" : ""); 
        window.Width = 840;
        window.Height = 560;


        return window;
    }
}