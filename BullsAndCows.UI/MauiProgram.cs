using BullsAndCows.Application.Interfaces;
using BullsAndCows.Application.Services;
using BullsAndCows.UI.ViewModels;
using BullsAndCows.UI.Views;
using Microsoft.Extensions.Logging;

namespace BullsAndCows.UI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddSingleton<IGameAppService, GameAppService>();

        builder.Services.AddTransient<GamePageViewModel>();
        builder.Services.AddTransient<GamePage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
