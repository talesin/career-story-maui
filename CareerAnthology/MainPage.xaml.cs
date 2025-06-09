using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenAI;
using StoryMaker;

namespace CareerAnthology
{
    public partial class MainPage : ContentPage
    {
        private readonly IConfiguration _config;
        private readonly ILogger<MainPage> _logger;
        private readonly string openAIApiKey;

        public MainPage(IConfiguration config, ILogger<MainPage> logger)
        {
            InitializeComponent();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            openAIApiKey = _config["openai_api_key"] ?? string.Empty;
        }

        // Display fields for StoryScore

        private void OnStoryTextChanged(object sender, TextChangedEventArgs e)
        {
            bool hasText = !string.IsNullOrWhiteSpace(Story.Text);
            Score.IsEnabled = hasText;
            //Review.IsEnabled = hasText;
        }

        private async void OnScoreClicked(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(openAIApiKey))
            {
                await DisplayAlert("Error", "OpenAI API key is not set. Please configure it in the app settings.", "OK");
                return;
            }

            OpenAIClient client = new (openAIApiKey);
            StoryEvaluator evaluator = new (client, _logger);
            var storyScore = await evaluator.Evaluate(Story.Text);

            if (storyScore != null)
            {
                RelevanceLabel.Text = $"{storyScore.Relevance.Score}: {storyScore.Relevance.Explanation}";
                OwnershipLabel.Text = $"{storyScore.Ownership.Score}: {storyScore.Ownership.Explanation}";
                ComplexityLabel.Text = $"{storyScore.Complexity.Score}: {storyScore.Complexity.Explanation}";
                InfluenceLabel.Text = $"{storyScore.Influence.Score}: {storyScore.Influence.Explanation}";
                OutcomeLabel.Text = $"{storyScore.Outcome.Score}: {storyScore.Outcome.Explanation}";
                ReflectionLabel.Text = $"{storyScore.Reflection.Score}: {storyScore.Reflection.Explanation}";
                AreasForImprovmentLabel.Text = string.Join(", ", storyScore.AreasForImprovment ?? new string[0]);
                TotalScoreLabel.Text = $"{storyScore.PercentageScore}/100";
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
