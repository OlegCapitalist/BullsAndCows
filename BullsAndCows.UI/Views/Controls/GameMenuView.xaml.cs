using BullsAndCows.UI.ViewModels;

namespace BullsAndCows.UI.Views.Controls;

public partial class GameMenuView : ContentView
{
	public GameMenuView()
	{
		InitializeComponent();
	}

    private async void OnNewGameClicked(object? sender, EventArgs e)
    {
        bool confirmed = await Shell.Current.DisplayAlert(
            "Новая игра",
            "Текущая игра будет завершена. Начать новую игру?",
            "Да",
            "Отмена");

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