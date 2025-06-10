using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CareerAnthology
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            var configBuilder = new ConfigurationBuilder()
                .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true);

            builder.Logging.AddConsole();

#if DEBUG
            configBuilder.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);
            builder.Logging.AddDebug();
#else
            configBuilder.AddJsonFile("appsettings.Production.json", optional: true, reloadOnChange: true);
#endif

            configBuilder.AddEnvironmentVariables();
            var config = configBuilder.Build();
            builder.Configuration.AddConfiguration(config);

            builder.Services.AddSingleton<MainPage>();

            return builder.Build();
        }
    }
}
