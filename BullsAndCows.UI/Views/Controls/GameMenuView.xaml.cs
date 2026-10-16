using BullsAndCows.UI.ViewModels;
using BullsAndCows.UI.Resources.Localization;

namespace BullsAndCows.UI.Views.Controls;

public partial class GameMenuView : ContentView
{
	public GameMenuView()
	{
		InitializeComponent();
	}

    private async void OnNewGameClicked(object? sender, EventArgs e)
    {
        bool confirmed = await Shell.Current.DisplayAlertAsync(
            AppResources.NewGame,
            AppResources.NewGameConfirmation,
            AppResources.Yes,
            AppResources.Cancel);

        if (!confirmed)
            return;

        if (BindingContext is GamePageViewModel viewModel)
        {
            await viewModel.StartGameAsync();
        }
    }

    private async void OnExitClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//HomePage");
    }

    private async void OnHelpClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(HelpPage));
    }
}