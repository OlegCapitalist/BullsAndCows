
using BullsAndCows.Application.Interfaces;
using BullsAndCows.UI.Interfaces;
using BullsAndCows.UI.Models;
using BullsAndCows.UI.Resources.Localization;
using BullsAndCows.UI.Services;
using PropertyChanged;
using System.Globalization;

namespace BullsAndCows.UI.ViewModels;

[AddINotifyPropertyChangedInterface]
public class HomePageViewModel
{
    private readonly ISettingsService _settingsService = null!;
    private readonly IThemeService _themeService = null!;
    private readonly ILocalizationService _localizationService = null!;

    public bool IsLoadingVisible { get; private set; } = true;
    public bool IsMenuVisible { get; private set; } = false;

    public HomePageViewModel(ISettingsService settingsService, IThemeService themeService, ILocalizationService localizationService)
    {
        _settingsService = settingsService;
        _themeService = themeService;
        _localizationService = localizationService;
    }

    public async Task InitializeAsync()
    {
        IsLoadingVisible = true;
        IsMenuVisible = false;

        var settings = await _settingsService.GetSettingsAsync();

        ApplyTheme(settings.Theme);

        ApplyLanguage(settings.Language);

        IsLoadingVisible = false;
        IsMenuVisible = true;
    }

    private void ApplyTheme(string themeString)
    {
        var theme = Enum.TryParse<Themes>(
            themeString,
            ignoreCase: true,
            out var parsedTheme)
                ? parsedTheme
                : Themes.System;

        _themeService.Apply(theme);
    }

    private void ApplyLanguage(string languageString)
    {
        var language = Enum.TryParse<Languages>(
            languageString,
            ignoreCase: true,
            out var parsedLanguage)
                ? parsedLanguage
                : Languages.English;
        _localizationService.Apply(language);

        var a1 = CultureInfo.CurrentUICulture.Name;
        var a2 = AppResources.Culture?.Name;
        var a3 = AppResources.Theme_System;
    }
}
