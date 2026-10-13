using BullsAndCows.UI.ViewModels;

namespace BullsAndCows.UI.Views;

public partial class GamePage : ContentPage
{
    private GamePageViewModel ViewModel =>
        (GamePageViewModel)BindingContext;

    public GamePage(GamePageViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await ViewModel.StartGameAsync();
    }

    protected void OnOrientationChanged(object? sender, EventArgs e)
    {
        bool isLandscape = (Width > Height);

        PortraitLayout.IsVisible = !isLandscape;
        LandscapeLayout.IsVisible = isLandscape;
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);

        bool isLandscape = (Width > Height);

        PortraitLayout.IsVisible = !isLandscape;
        LandscapeLayout.IsVisible = isLandscape;
    }
}
