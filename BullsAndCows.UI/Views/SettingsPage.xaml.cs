using BullsAndCows.UI.Models;
using BullsAndCows.UI.ViewModels;

namespace BullsAndCows.UI.Views;

public partial class SettingsPage : ContentPage
{
    public IReadOnlyList<SelectionItem<Languages>> AvailableLanguages { get; } =
    [
        new(Languages.Russian, "Русский"),
        new(Languages.English, "English"),
        new(Languages.QazaqCyrillic, "Қазақша"),
        new(Languages.QazaqLatin, "Qazaqşa")
    ];
    public SelectionItem<Languages>? SelectedLanguage { get; set; } = null;

    public IReadOnlyList<SelectionItem<Themes>> AvailableThemes { get; } =
    [
        new(Themes.System, "Системная"),
        new(Themes.Light, "Светлая"),
        new(Themes.Dark, "Темная")
    ];
    public SelectionItem<Themes>? SelectedTheme { get; set; } = null;

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