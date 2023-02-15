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

//            builder.ConfigureLifecycleEvents(AppLifecycle =>
//            {

//#if WINDOWS
//                AppLifecycle.AddWindows(windows => windows.OnWindowCreated((window) =>
//                {
//                    window.ExtendsContentIntoTitleBar = true;
//                }));
//#endif
//            });
            builder.UseMauiCommunityToolkit();
            builder.Services.AddSingleton<AuthenticationPage>();
            builder.Services.AddSingleton<AuthService>();

            builder.ConfigureLifecycleEvents(lifecycle =>
            {
            });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}