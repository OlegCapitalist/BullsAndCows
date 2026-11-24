namespace BullsAndCows.UI.Views.Controls;

public partial class WonView : ContentView
{
	public WonView()
	{
		InitializeComponent();
	}

    private async void OnRecordsClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(RecordsPage));
    }
}