using BullsAndCows.Application.Extensions;
using BullsAndCows.Application.Interfaces;
using BullsAndCows.Application.Services;
using BullsAndCows.Infrastructure.Storage;
using BullsAndCows.UI.Interfaces;
using BullsAndCows.UI.ViewModels;
using BullsAndCows.UI.Views;
using Microsoft.Extensions.Logging;
using BullsAndCows.UI.Services;

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

        builder.Services.AddApplication();

        var settingsFilePath = Path.Combine(
            FileSystem.AppDataDirectory,
            "appsettings.json");

        builder.Services.AddSingleton<ISettingsStorage>(
            new JsonSettingsStorage(
                settingsFilePath,
                _ => FileSystem.OpenAppPackageFileAsync("appsettings.json")));

        builder.Services.AddSingleton<ISettingsService, SettingsService>();
        builder.Services.AddSingleton<IThemeService, ThemeService>();

        builder.Services.AddTransient<GamePageViewModel>();
        builder.Services.AddTransient<GamePage>();

        builder.Services.AddTransient<HomePageViewModel>();
        builder.Services.AddTransient<HomePage>();

        builder.Services.AddTransient<SettingsPageViewModel>();
        builder.Services.AddTransient<SettingsPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
