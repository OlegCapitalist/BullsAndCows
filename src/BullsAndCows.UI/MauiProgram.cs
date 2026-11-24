using BullsAndCows.Application.Extensions;
using BullsAndCows.Application.Interfaces;
using BullsAndCows.Application.Services;
using BullsAndCows.Infrastructure.DataBase;
using BullsAndCows.Infrastructure.Storage;
using BullsAndCows.UI.Interfaces;
using BullsAndCows.UI.Services;
using BullsAndCows.UI.ViewModels;
using BullsAndCows.UI.Views;
using Microsoft.Extensions.Logging;
using BullsAndCows.Infrastructure.Repositories;
using AbstractionsIFileSystem = System.IO.Abstractions.IFileSystem;
using AbstractionsFileSystem = System.IO.Abstractions.FileSystem;
using MauiFileSystem = Microsoft.Maui.Storage.FileSystem;

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

        builder.Services.AddSingleton<AbstractionsIFileSystem, AbstractionsFileSystem>();

        var settingsFilePath = Path.Combine(
            MauiFileSystem.AppDataDirectory,
            "appsettings.json");


        builder.Services.AddSingleton<ISettingsStorage>(sp =>
            new JsonSettingsStorage(
                settingsFilePath,
                _ => MauiFileSystem.OpenAppPackageFileAsync("appsettings.json"),
                sp.GetRequiredService<AbstractionsIFileSystem>()));

        builder.Services.AddSingleton(sp =>
        new GameDatabase(
            sp.GetRequiredService<ISettingsStorage>(),
            MauiFileSystem.AppDataDirectory));

        builder.Services.AddSingleton<ISettingsService, SettingsService>();
        builder.Services.AddSingleton<IThemeService, ThemeService>();
        builder.Services.AddSingleton<AbstractionsIFileSystem, AbstractionsFileSystem>();

        builder.Services.AddSingleton<IGameRecordRepository, Repository>();

        builder.Services.AddSingleton<ILocalizationService, LocalizationService>();

        builder.Services.AddTransient<GamePageViewModel>();
        builder.Services.AddTransient<GamePage>();

        builder.Services.AddTransient<SettingsPageViewModel>();
        builder.Services.AddTransient<SettingsPage>();

        builder.Services.AddTransient<HelpPageViewModel>();
        builder.Services.AddTransient<HelpPage>();

        builder.Services.AddTransient<RecordsPageViewModel>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
