using BullsAndCows.UI.Views;

namespace BullsAndCows.UI;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(HelpPage), typeof(HelpPage));
    }
}
