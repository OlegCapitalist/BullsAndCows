using BullsAndCows.Application.DTOs;
using BullsAndCows.Application.Interfaces;
using BullsAndCows.Application.Models;
using BullsAndCows.Application.Services;
using BullsAndCows.UI.Interfaces;
using BullsAndCows.UI.Models;
using BullsAndCows.UI.Resources.Localization;
using PropertyChanged;
using System.Windows.Input;

namespace BullsAndCows.UI.ViewModels;

[AddINotifyPropertyChangedInterface]
public class SettingsPageViewModel
{
    private readonly ISettingsService _settingsService = null!;

    private readonly IThemeService _themeService = null!;
    private readonly ILocalizationService _localizationService = null!;

    private Themes _oldTheme;
    private Languages _oldLanguage;

    public int CodeLength { get; set; }

    public int MaxAttempts { get; set; }

    
    public int SelectedThemeIndex
    { 
        get => Array.IndexOf(Enum.GetValues(typeof(Themes)), SelectedTheme);
        set
        {
            var values = Enum.GetValues<Themes>();

            if (value >= 0 && value < values.Length)
                SelectedTheme = values[value];
        }
    }

    public int SelectedLanguageIndex
    {
        get => Array.IndexOf(Enum.GetValues(typeof(Languages)), SelectedLanguage);
        set
        {
            var values = Enum.GetValues<Languages>();

            if (value >= 0 && value < values.Length)
                SelectedLanguage = values[value];
        }
    }

    public IReadOnlyList<SelectionItem<Themes>> AvailableThemes { get; set; } = [];

    public IReadOnlyList<SelectionItem<Languages>> AvailableLanguages { get; set; } = [
            new(Languages.English, "English"),
            new(Languages.Russian, "Русский"),
            new(Languages.QazaqCyrillic, "Қазақша"),
            new(Languages.QazaqLatin, "Qazaqşa")
        ];

    public Themes SelectedTheme { get; set; }

    public Languages SelectedLanguage { get; set; }

    public ICommand SaveSettingsCommand { get; }

    public SettingsPageViewModel(
        ISettingsService settingsService, 
        IThemeService themeService, 
        ILocalizationService localizationService)
    {
        _settingsService = settingsService;
        _themeService = themeService;
        _localizationService = localizationService;
        _oldTheme = _themeService.CurrentTheme;
        _oldLanguage = _localizationService.CurrentLanguage;

        SaveSettingsCommand = new Command(async () => await SaveSettingsAsync());
    }   

    public async Task LoadSettingsAsync()
    {
        var settings = await _settingsService.GetSettingsAsync();

        CodeLength = settings.CodeLength;
        MaxAttempts = settings.MaxAttempts;

        var selectedTheme = Enum.Parse<Themes>(settings.Theme, true);
        var selectedLanguage = Enum.Parse<Languages>(settings.Language, true);

        //if (SelectedLanguage != _oldLanguage)
            _localizationService.Apply(selectedLanguage);

        if (SelectedTheme != _oldTheme)
            _themeService.Apply(selectedTheme);
        
        AvailableThemes = GetAvailableThemes();
        SelectedTheme = selectedTheme;
        SelectedLanguage = selectedLanguage;
    }

    private async Task SaveSettingsAsync()
    {
        if (SelectedTheme != _oldTheme)
            _themeService.Apply(SelectedTheme);

        if (SelectedLanguage != _oldLanguage)
            _localizationService.Apply(SelectedLanguage);

        await _settingsService.SaveSettingsAsync(
            new UserSettings(CodeLength, MaxAttempts, SelectedTheme.ToString(), SelectedLanguage.ToString())
        );

        await Shell.Current.GoToAsync("//HomePage");
    }

    private static IReadOnlyList<SelectionItem<Themes>> GetAvailableThemes()
    {
        var themes = Enum.GetValues<Themes>()
            .Select(theme => new SelectionItem<Themes>(
                theme,
                GetThemeName(theme)))
            .ToArray();

        return themes;
    }

    private static string GetThemeName(Themes theme)
    {
        var name = theme switch
        {
            Themes.System => AppResources.Theme_System,
            Themes.Light => AppResources.Theme_Light,
            Themes.Dark => AppResources.Theme_Dark,
            _ => theme.ToString()
        };
        return name;
    }
}
