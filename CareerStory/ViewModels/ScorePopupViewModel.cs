using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StoryMaker;

namespace CareerStory.ViewModels;

public partial class ScorePopupViewModel : BaseViewModel
{
    [ObservableProperty]
    private string _relevanceText = string.Empty;

    [ObservableProperty]
    private string _ownershipText = string.Empty;

    [ObservableProperty]
    private string _complexityText = string.Empty;

    [ObservableProperty]
    private string _influenceText = string.Empty;

    [ObservableProperty]
    private string _outcomeText = string.Empty;

    [ObservableProperty]
    private string _reflectionText = string.Empty;

    [ObservableProperty]
    private string _areasForImprovementText = string.Empty;

    [ObservableProperty]
    private string _overallText = string.Empty;

    [ObservableProperty]
    private int _totalScore;

    [ObservableProperty]
    private int _percentageScore;

    public event EventHandler? CloseRequested;

    public ScorePopupViewModel(StoryScore storyScore)
    {
        Title = "Story Evaluation Results";
        LoadStoryScore(storyScore);
    }

    private void LoadStoryScore(StoryScore storyScore)
    {
        RelevanceText = FormatCriteria(storyScore.Relevance);
        OwnershipText = FormatCriteria(storyScore.Ownership);
        ComplexityText = FormatCriteria(storyScore.Complexity);
        InfluenceText = FormatCriteria(storyScore.Influence);
        OutcomeText = FormatCriteria(storyScore.Outcome);
        ReflectionText = FormatCriteria(storyScore.Reflection);
        
        AreasForImprovementText = string.Join(
            Environment.NewLine + "• ", 
            storyScore.AreasForImprovement ?? Array.Empty<string>());
        
        if (!string.IsNullOrEmpty(AreasForImprovementText))
        {
            AreasForImprovementText = "• " + AreasForImprovementText;
        }

        TotalScore = storyScore.TotalScore;
        PercentageScore = storyScore.PercentageScore;
        OverallText = $"{storyScore.Overall} ({PercentageScore}/100)";
    }

    private static string FormatCriteria(Criteria criteria) =>
        $"{criteria.Score}: {criteria.Explanation}";

    [RelayCommand]
    private void Close()
    {
        CloseRequested?.Invoke(this, EventArgs.Empty);
    }
}