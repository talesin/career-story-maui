using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenAI;
using StoryMaker;

namespace CareerStory
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

            var openAIApiKey = config["openai_api_key"] ?? throw new InvalidOperationException("OpenAI API key is not configured in the app settings.");


            builder.Services
                .AddSingleton<MainPage>()
                .AddSingleton<IStoryEvaluator, StoryEvaluator>()
                .AddSingleton<IChatManager, OpenAIChatManager>()
                .AddSingleton(new OpenAIClient(openAIApiKey));
            

            return builder.Build();
        }
    }
}
