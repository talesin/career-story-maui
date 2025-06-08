using Microsoft.Extensions.Logging;
using OpenAI;
using Microsoft.Extensions.Configuration;
using StoryMaker;

namespace CareerAnthology
{
    public partial class MainPage : ContentPage
    {
        private readonly string openAIApiKey;

        public MainPage()
        {
            InitializeComponent();
            openAIApiKey = App.Current?.Handler?.MauiContext?.Services?.GetService<IConfiguration>()?["openai_api_key"] ?? string.Empty;
        }

        private async void OnScoreClicked(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(openAIApiKey))
            {
                await DisplayAlert("Error", "OpenAI API key is not set. Please configure it in the app settings.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(Story.Text))
            {
                await DisplayAlert("Error", "Please enter both a story", "OK");
                return;
            }

            using ILoggerFactory factory = LoggerFactory.Create(builder => builder
                .AddDebug()
                .AddConsole());
            var logger = factory.CreateLogger<MainPage>();

            OpenAIClient client = new (openAIApiKey);
            StoryEvaluator evaluator = new (client, logger);
            var result = await evaluator.Evaluate(Story.Text);

            await DisplayAlert("Acknowledgement", result?.ToString(), "OK");
        }

        private async void OnReviewClicked(object? sender, EventArgs e)
        {
            await DisplayAlert("Acknowledgement", "Review button clicked!", "OK");
        }
    }
}
