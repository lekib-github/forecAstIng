using forecAstIng.View;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using forecAstIng.Services;

namespace forecAstIng
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton<DataService>();

            builder.Services.AddSingleton<ForecastsViewModel>();
            builder.Services.AddSingleton<MainPage>();

            builder.Services.AddTransient<MorePageViewModel>();
            builder.Services.AddTransient<MorePage>();

            #if DEBUG
                builder.Logging.AddDebug();
            #endif

            return builder.Build();
        }
    }
}
