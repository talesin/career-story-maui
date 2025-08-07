using StoryMaker;

namespace CareerStory.Services;

public class NavigationService : INavigationService
{
    public async Task ShowScorePopupAsync(StoryScore storyScore)
    {
        var popup = new ScorePopup(storyScore);
        
        if (Application.Current?.MainPage is not null)
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(popup);
        }
    }

    public async Task DisplayAlertAsync(string title, string message, string cancel)
    {
        if (Application.Current?.MainPage is not null)
        {
            await Application.Current.MainPage.DisplayAlert(title, message, cancel);
        }
    }
}