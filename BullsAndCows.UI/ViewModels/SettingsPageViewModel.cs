using BullsAndCows.Application.DTOs;
using BullsAndCows.Application.Models;
using BullsAndCows.Application.Interfaces;
using BullsAndCows.Application.Services;
using BullsAndCows.UI.Interfaces;
using BullsAndCows.UI.Models;
using PropertyChanged;
using System.Windows.Input;

namespace BullsAndCows.UI.ViewModels;

[AddINotifyPropertyChangedInterface]
public class SettingsPageViewModel
{
    private readonly ISettingsService _settingsService = null!;
    private readonly IThemeService _themeService = null!;
    private Themes _oldTheme;

    private readonly Dictionary<string, string> _settingsMock = new()
    {
        { "Language_English", "English" },
        { "Language_Russian", "Русский" },
        { "Language_QazaqCyrillic", "Қазақша" },
        { "Language_QazaqLatin", "Qazaqşa" },
        { "Theme_System", "Системная" },
        { "Theme_Light", "Светлая" },
        { "Theme_Dark", "Темная" }
    };

    public int CodeLength { get; set; } = 4;

    public int MaxAttempts { get; set; } = 10;

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

    public IReadOnlyList<SelectionItem<Themes>> AvailableThemes { get; }

    public IReadOnlyList<SelectionItem<Languages>> AvailableLanguages { get; } 

    public Themes SelectedTheme { get; set; }

    public Languages SelectedLanguage { get; set; }

    public ICommand SaveSettingsCommand { get; }

    public SettingsPageViewModel(ISettingsService settingsService, IThemeService themeService)
    {
        _settingsService = settingsService;
        _themeService = themeService;
        _oldTheme = _themeService.CurrentTheme;

        SaveSettingsCommand = new Command(async () => await SaveSettingsAsync());

        AvailableLanguages = [
            new(Languages.English, _settingsMock["Language_English"]),
            new(Languages.Russian, _settingsMock["Language_Russian"]),
            new(Languages.QazaqCyrillic, _settingsMock["Language_QazaqCyrillic"]),
            new(Languages.QazaqLatin, _settingsMock["Language_QazaqLatin"])
        ];

        AvailableThemes = [
            new(Themes.System, _settingsMock["Theme_System"]),
            new(Themes.Light, _settingsMock["Theme_Light"]),
            new(Themes.Dark, _settingsMock["Theme_Dark"])
        ];
    }   

    public async Task LoadSettingsAsync()
    {
        var settings = await _settingsService.GetSettingsAsync();
        CodeLength = settings.CodeLength;
        MaxAttempts = settings.MaxAttempts;

        SelectedTheme = Enum.Parse<Themes>(settings.Theme, true);
        SelectedLanguage = Enum.Parse<Languages>(settings.Language, true);

        if (SelectedTheme != _oldTheme)
            _themeService.Apply(SelectedTheme);
    }

    private async Task SaveSettingsAsync()
    {
        if (SelectedTheme != _oldTheme)
            _themeService.Apply(SelectedTheme);

        await _settingsService.SaveSettingsAsync(
            new UserSettings(CodeLength, MaxAttempts, SelectedTheme.ToString(), SelectedLanguage.ToString())
        );

        await Shell.Current.GoToAsync("//HomePage");
    }


}
