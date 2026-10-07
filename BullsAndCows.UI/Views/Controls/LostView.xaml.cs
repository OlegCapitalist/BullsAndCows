namespace BullsAndCows.UI.Views.Controls;

public partial class LostView : ContentView
{
	public LostView()
	{
        InitializeComponent();
	}

    private async void OnRecordsClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(RecordsPage));
    }
}