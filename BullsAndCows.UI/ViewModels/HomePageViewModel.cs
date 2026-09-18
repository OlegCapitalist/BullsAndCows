
using BullsAndCows.Application.Interfaces;
using BullsAndCows.UI.Interfaces;
using BullsAndCows.UI.Models;
using PropertyChanged;

namespace BullsAndCows.UI.ViewModels;

[AddINotifyPropertyChangedInterface]
public class HomePageViewModel
{
    private readonly ISettingsService _settingsService = null!;
    private readonly IThemeService _themeService = null!;

    public bool IsLoadingVisible { get; private set; } = true;
    public bool IsMenuVisible { get; private set; } = false;

    public HomePageViewModel(ISettingsService settingsService, IThemeService themeService)
    {
        _settingsService = settingsService;
        _themeService = themeService;
    }

    public async Task InitializeAsync()
    {
        IsLoadingVisible = true;
        IsMenuVisible = false;

        var settings = await _settingsService.GetSettingsAsync();

        var theme = Enum.TryParse<Themes>(
            settings.Theme,
            ignoreCase: true,
            out var parsedTheme)
                ? parsedTheme
                : Themes.System;

        _themeService.Apply(theme);

        IsLoadingVisible = false;
        IsMenuVisible = true;
    }


}
