using BullsAndCows.UI.ViewModels;

namespace BullsAndCows.UI.Views;

public partial class RecordsPage : ContentPage
{
    private RecordsPageViewModel ViewModel =>
        (RecordsPageViewModel)BindingContext;
    public RecordsPage(RecordsPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await ViewModel.LoadRecordsAsync();
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}