using Microsoft.Extensions.DependencyInjection;

namespace BullsAndCows.UI;

public partial class App : MauiApplication
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell())
        {
            Title = "Bulls and Cows",

            Width = 390,
            Height = 844,

            MinimumWidth = 360,
            MinimumHeight = 700

        };

    }

    
}