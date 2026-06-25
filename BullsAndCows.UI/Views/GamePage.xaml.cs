using System.Windows.Input;
using BullsAndCows.UI.ViewModels;

namespace BullsAndCows.UI.Views;

public partial class GamePage : ContentPage
{
    int count = 0;

    public GamePage()
    {
        InitializeComponent();

        BindingContext = new GamePageViewModel(); ;
    }

    private void OnCounterClicked(object? sender, EventArgs e)
    {
        count++;

        if (count == 1)
            CounterBtn.Text = $"Clicked {count} time";
        else
            CounterBtn.Text = $"Clicked {count} times";

        SemanticScreenReader.Announce(CounterBtn.Text);
    }
    private void OnNewGameClicked(object? sender, EventArgs e)
    {
        // Implementation for new game clicked
    }

    private void OnExitClicked(object? sender, EventArgs e)
    {
        // Implementation for exit clicked
    }

    private void OnRulesClicked(object? sender, EventArgs e)
    {
        // Implementation for exit clicked
    }


    
}
