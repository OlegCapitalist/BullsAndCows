using BullsAndCows.UI.ViewModels;

namespace BullsAndCows.UI.Views;

public partial class HelpPage : ContentPage
{
    private HelpPageViewModel ViewModel =>
        (HelpPageViewModel)BindingContext;

    public HelpPage(HelpPageViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await ViewModel.LoadRulesAsync();
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}