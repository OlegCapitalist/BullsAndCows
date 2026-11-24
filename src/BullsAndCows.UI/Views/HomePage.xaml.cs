
namespace BullsAndCows.UI.Views;

public partial class HomePage : ContentPage
{
    public HomePage()
	{
		InitializeComponent();

    }

    private async void OnStartGameClicked(object sender, EventArgs e)
	{
        await Shell.Current.GoToAsync("//GamePage");
    }

	private async void OnSettingsClicked(object sender, EventArgs e)
	{
        await Shell.Current.GoToAsync("//SettingsPage");
    }

	//private async void Button_Clicked(object sender, EventArgs e)
	//{
 //       // Records / leaderboard — not implemented, show placeholder page
 //       await DisplayAlert("Настройки", "Страница настроек пока не реализована.", "ОК");
 //   }

    private async void OnRecordsClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(RecordsPage));
    }

    private async void OnHelpClicked(object sender, EventArgs e)
	{
        await Shell.Current.GoToAsync(nameof(HelpPage));
    }
}
