using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using StoryMaker;
using CareerStory.Services;
using LanguageExt;
using static LanguageExt.Prelude;

namespace CareerStory.ViewModels;

public partial class MainPageViewModel : BaseViewModel
{
    private readonly IStoryEvaluator _storyEvaluator;
    private readonly INavigationService _navigationService;
    private readonly ILogger<MainPageViewModel> _logger;

    [ObservableProperty]
    private string _storyText = string.Empty;

    [ObservableProperty]
    private bool _canScore;

    [ObservableProperty]
    private bool _canReview;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _hasError;

    private Option<StoryScore> _cachedStoryScore = None;

    public MainPageViewModel(
        IStoryEvaluator storyEvaluator,
        INavigationService navigationService,
        ILogger<MainPageViewModel> logger)
    {
        _storyEvaluator = storyEvaluator;
        _navigationService = navigationService;
        _logger = logger;
        Title = "Career Story Evaluator";
    }

    partial void OnStoryTextChanged(string value)
    {
        var hasValidText = ValidateStoryText(value).IsRight;
        CanScore = hasValidText;
        CanReview = hasValidText;
        
        // Clear cached score when text changes
        _cachedStoryScore = None;
        
        // Clear any existing errors when user starts typing
        if (HasError && !string.IsNullOrWhiteSpace(value))
        {
            ClearError();
        }
    }

    [RelayCommand]
    private async Task ScoreStoryAsync()
    {
        var validationResult = ValidateStoryText(StoryText);
        
        await validationResult.Match(
            Left: ShowValidationError,
            Right: async validText => await EvaluateAndShowScore(validText)
        );
    }

    [RelayCommand]
    private async Task ReviewStoryAsync()
    {
        // Placeholder for future review functionality
        await _navigationService.DisplayAlertAsync("Coming Soon", "Review functionality will be implemented soon!", "OK");
    }

    private Either<string, string> ValidateStoryText(string text) =>
        string.IsNullOrWhiteSpace(text) ? Left("Please enter your story before scoring.") :
        text.Length < 50 ? Left("Story must be at least 50 characters long.") :
        text.Length > 5000 ? Left("Story must be less than 5000 characters long.") :
        Right(text);

    private Task ShowValidationError(string error)
    {
        ErrorMessage = error;
        HasError = true;
        return Task.CompletedTask;
    }

    private async Task EvaluateAndShowScore(string validStoryText)
    {
        IsBusy = true;
        ClearError();

        try
        {
            var scoreResult = await _cachedStoryScore.Match(
                Some: score => Task.FromResult(Optional(score)),
                None: async () => await EvaluateStory(validStoryText)
            );

            await scoreResult.Match(
                Some: async score => await _navigationService.ShowScorePopupAsync(score),
                None: () => ShowError("Failed to evaluate story. Please check your internet connection and try again.")
            );
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task<Option<StoryScore>> EvaluateStory(string storyText)
    {
        try
        {
            _logger.LogInformation("Evaluating story with {CharacterCount} characters", storyText.Length);
            
            var score = await _storyEvaluator.Evaluate(storyText);
            var result = Optional(score);
            
            // Cache the result
            _cachedStoryScore = result;
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error evaluating story");
            return None;
        }
    }

    private Task ShowError(string message)
    {
        ErrorMessage = message;
        HasError = true;
        return Task.CompletedTask;
    }

    private void ClearError()
    {
        ErrorMessage = string.Empty;
        HasError = false;
    }
}