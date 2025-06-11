using Microsoft.Maui.Controls;
using StoryMaker;
using System;

namespace CareerAnthology
{
    public partial class ScorePopup : ContentPage
    {
        public ScorePopup(StoryScore storyScore)
        {
            InitializeComponent();
            RelevanceLabel.Text = FormatCriteria(storyScore.Relevance);
            OwnershipLabel.Text = FormatCriteria(storyScore.Ownership);
            ComplexityLabel.Text = FormatCriteria(storyScore.Complexity);
            InfluenceLabel.Text = FormatCriteria(storyScore.Influence);
            OutcomeLabel.Text = FormatCriteria(storyScore.Outcome);
            ReflectionLabel.Text = FormatCriteria(storyScore.Reflection);
            AreasForImprovmentLabel.Text = string.Join(Environment.NewLine, storyScore.AreasForImprovment ?? Array.Empty<string>());
            OverallLabel.Text = $"{storyScore.Overall} ({storyScore.PercentageScore}/100)";
        }

        private static string FormatCriteria(Criteria criteria)
        {
            return $"{criteria.Score}: {criteria.Explanation}";
        }

        private async void OnCloseClicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}
