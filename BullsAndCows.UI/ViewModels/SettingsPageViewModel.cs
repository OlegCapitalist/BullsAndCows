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

    private SelectionItem<Themes>? _selectedThemeItem;
    private SelectionItem<Languages>? _selectedLanguageItem;

    public int CodeLength { get; set; }

    public int MaxAttempts { get; set; }
    
    public SelectionItem<Themes>? SelectedThemeItem
    {
        get { return _selectedThemeItem; }
        set
        {
            _selectedThemeItem = value;

            if (value is not null)
                SelectedTheme = value.Value;
        }
    }

    public SelectionItem<Languages>? SelectedLanguageItem
    {
        get { return _selectedLanguageItem; }
        set
        {
            _selectedLanguageItem = value;
            if (value is not null)
                SelectedLanguage = value.Value;
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

        if (_oldLanguage != selectedLanguage)
            _localizationService.Apply(selectedLanguage);

        if (_oldTheme != selectedTheme)
            _themeService.Apply(selectedTheme);

        AvailableThemes = GetAvailableThemes();

        SelectedThemeItem = AvailableThemes.First(x => x.Value == selectedTheme);
        SelectedLanguageItem = AvailableLanguages.First(x => x.Value == selectedLanguage);

        _oldLanguage = selectedLanguage;
        _oldTheme = selectedTheme;
    }

    private async Task SaveSettingsAsync()
    {
        if (SelectedLanguage != _oldLanguage)
            _localizationService.Apply(SelectedLanguage);

        if (SelectedTheme != _oldTheme)
            _themeService.Apply(SelectedTheme);

        await _settingsService.SaveSettingsAsync(
            new UserSettings(CodeLength, MaxAttempts, SelectedTheme.ToString(), SelectedLanguage.ToString())
        );

        if (_oldLanguage != SelectedLanguage)
        {
            var window = MauiControlsApplication.Current?.Windows.FirstOrDefault();

            if (window is not null)
                window.Page = new AppShell();
        }

        _oldLanguage = SelectedLanguage;
        _oldTheme = SelectedTheme;

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
