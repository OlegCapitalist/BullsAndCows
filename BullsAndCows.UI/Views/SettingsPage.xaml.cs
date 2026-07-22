using BullsAndCows.UI.ViewModels;

namespace BullsAndCows.UI.Views;

public partial class SettingsPage : ContentPage
{
	public SettingsPage(SettingsPageViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}