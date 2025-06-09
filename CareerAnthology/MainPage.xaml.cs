using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenAI;
using StoryMaker;
using System;
using Microsoft.Maui.Controls;

namespace CareerAnthology
{
    public partial class MainPage : ContentPage
    {
        private readonly IConfiguration config;
        private readonly ILogger<MainPage> logger;
        private readonly string openAIApiKey;
        private bool hasStoryChanged = false;
        private StoryScore? storyScore = null;

        public MainPage(IConfiguration config, ILogger<MainPage> logger)
        {
            InitializeComponent();
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.config = config ?? throw new ArgumentNullException(nameof(config));
            openAIApiKey = this.config["openai_api_key"] ?? throw new InvalidOperationException("OpenAI API key is not configured in the app settings."); 
        }


        private void OnStoryTextChanged(object sender, TextChangedEventArgs e)
        {
            bool hasText = !string.IsNullOrWhiteSpace(Story.Text);
            Score.IsEnabled = hasText;
            //Review.IsEnabled = hasText;
            hasStoryChanged = true; // Track if the story text has changed
        }

        private async void OnScoreClicked(object? sender, EventArgs e)
        {
            if (hasStoryChanged)
            {
                OpenAIClient client = new(openAIApiKey);
                StoryEvaluator evaluator = new(client, logger);
                storyScore = await evaluator.Evaluate(Story.Text); 
                hasStoryChanged = false; // Reset the change tracker
            }

            if (storyScore != null)
            {
                var popup = new ScorePopup(storyScore);
                await Navigation.PushModalAsync(popup);
            }
            else
            {
                await DisplayAlert("Error", "Failed to evaluate story.", "OK");
            }
        }

        private async void OnReviewClicked(object? sender, EventArgs e)
        {
            await DisplayAlert("Acknowledgement", "Review button clicked!", "OK");
        }
     }
    

}
