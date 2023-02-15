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


    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
//#if WINDOWS
//        var uiSettings = new Windows.UI.ViewManagement.UISettings();
//        var color = uiSettings.GetColorValue(Windows.UI.ViewManagement.UIColorType.AccentDark2);
//        Microsoft.UI.Xaml.Window window = (Microsoft.UI.Xaml.Window)App.Current.Windows.First<Window>().Handler.PlatformView;
//        IntPtr windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(window);
//        Microsoft.UI.WindowId windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(windowHandle);
//        Microsoft.UI.Windowing.AppWindow appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
//        Microsoft.UI.Windowing.AppWindowTitleBar titlebar = appWindow.TitleBar;
//        titlebar.BackgroundColor = color;
//        titlebar.ButtonBackgroundColor = color;
//#endif
    }

    private void newWindow_Clicked(object sender, EventArgs e)
    {
        Window secondWindow = new Window(new MainPage());
        Application.Current.OpenWindow(secondWindow);
    }
}