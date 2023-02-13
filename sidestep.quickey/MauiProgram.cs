using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using sidestep.quickey.Services;

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
            builder.UseMauiCommunityToolkit();

            builder.Services.AddSingleton<AuthenticationPage>();
            //builder.Services.AddTransient<SecretsPage>();
            builder.Services.AddSingleton<AuthService>();

            //builder.Services.AddSingleton<HttpClientFactory>();

            //builder.Services.AddHttpClient("ClientName", (opt) =>
            //        opt.BaseAddress = new Uri("<Add your API URL here>"))
            //.AddHttpMessageHandler((s) => s.GetService<AuthHandler>());


#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}