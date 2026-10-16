using BullsAndCows.Application.Interfaces;
using BullsAndCows.UI.Interfaces;
using BullsAndCows.UI.Models;
using Microsoft.Extensions.DependencyInjection;

namespace BullsAndCows.UI;

public partial class App : MauiApplication
{
    private readonly ISettingsService _settingsService;
    private readonly IThemeService _themeService;
    private readonly ILocalizationService _localizationService;

    public App(
        ISettingsService settingsService,
        IThemeService themeService,
        ILocalizationService localizationService)
    {
        InitializeComponent();

        _settingsService = settingsService;
        _themeService = themeService;
        _localizationService = localizationService;
    }

    private async Task InitializeAsync(Window window)
    {
        var settings = await _settingsService.GetSettingsAsync();

        var language = Enum.Parse<Languages>(
            settings.Language,
            true);

        var theme = Enum.Parse<Themes>(
            settings.Theme,
            true);

        _localizationService.Apply(language);
        _themeService.Apply(theme);

        window.Page = new AppShell();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = new Window(new ContentPage())
        {
            Width = 390,
            Height = 844,
            MinimumWidth = 360,
            MinimumHeight = 700
        };

        _ = InitializeAsync(window);

        return window;
    }

    
}