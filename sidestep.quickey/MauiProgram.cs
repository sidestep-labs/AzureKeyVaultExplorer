using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using sidestep.quickey.Services;

#if WINDOWS
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Windows.Graphics;
#endif

namespace sidestep.quickey
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder.UseMauiApp<App>().ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.ConfigureLifecycleEvents(AppLifecycle =>
            {
#if WINDOWS
                AppLifecycle.AddWindows(windows => windows.OnWindowCreated((window) =>
                {
                    window.ExtendsContentIntoTitleBar = false;
                    //window.SetTitleBar(Controls.CustomNavigationBar);

                    var uiSettings = new Windows.UI.ViewManagement.UISettings();
                    var color = uiSettings.GetColorValue(Windows.UI.ViewManagement.UIColorType.Background);
                    Microsoft.UI.Xaml.Window _window = (Microsoft.UI.Xaml.Window)App.Current.Windows.First<Window>().Handler.PlatformView;
                    //get the current window on the windows platform
                    IntPtr windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(_window);
                    Microsoft.UI.WindowId windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(windowHandle);
                    Microsoft.UI.Windowing.AppWindow appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
                    Microsoft.UI.Windowing.AppWindowTitleBar titlebar = appWindow.TitleBar;
                    //titlebar.ExtendsContentIntoTitleBar = true;
                    //You may need to uncomment the line above
                   //TODO: Get this from config
                    titlebar.BackgroundColor = color;
                    titlebar.InactiveBackgroundColor = color;
                    titlebar.ButtonBackgroundColor = color;
                    titlebar.ButtonInactiveBackgroundColor = color;
                }));
#endif
            });

            builder.UseMauiCommunityToolkit();
            builder.Services.AddSingleton<AuthenticationPage>();
            builder.Services.AddSingleton<AuthService>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}