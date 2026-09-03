using BullsAndCows.UI.ViewModels;

namespace BullsAndCows.UI.Views;

public partial class SettingsPage : ContentPage
{
    private SettingsPageViewModel ViewModel =>
        (SettingsPageViewModel)BindingContext;

    public SettingsPage(SettingsPageViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await ViewModel.LoadSettingsAsync();
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//HomePage");
    }

}