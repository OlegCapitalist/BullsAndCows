using BullsAndCows.UI.ViewModels;

namespace BullsAndCows.UI.Views;

public partial class GamePage : ContentPage
{
    public GamePage()
    {
        InitializeComponent();

        BindingContext = new GamePageViewModel();
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

    private void OnNewGameClicked(object? sender, EventArgs e)
    {
        // Implementation for new game clicked
    }

    private void OnExitClicked(object? sender, EventArgs e)
    {
        // Implementation for exit clicked
    }

    private void OnRulesClicked(object? sender, EventArgs e)
    {
        // Implementation for exit clicked
    }


    
}
