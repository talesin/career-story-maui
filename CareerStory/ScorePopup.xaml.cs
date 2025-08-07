using CareerStory.ViewModels;
using StoryMaker;

namespace CareerStory;

public partial class ScorePopup : ContentPage
{
    private readonly ScorePopupViewModel _viewModel;

    public ScorePopup(StoryScore storyScore)
    {
        InitializeComponent();
        _viewModel = new ScorePopupViewModel(storyScore);
        BindingContext = _viewModel;
        
        // Subscribe to the close event
        _viewModel.CloseRequested += OnCloseRequested;
    }

    private async void OnCloseRequested(object? sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        // Unsubscribe to prevent memory leaks
        _viewModel.CloseRequested -= OnCloseRequested;
    }
}
