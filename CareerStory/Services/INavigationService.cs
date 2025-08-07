using StoryMaker;

namespace CareerStory.Services;

public interface INavigationService
{
    Task ShowScorePopupAsync(StoryScore storyScore);
    Task DisplayAlertAsync(string title, string message, string cancel);
}