using BullsAndCows.Application.DTOs;
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

    private readonly Dictionary<string, string> localizationMock = new()
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

        SaveSettingsCommand = new Command(async () => await SaveSettingsAsync());

        AvailableLanguages = [
            new(Languages.English, localizationMock["Language_English"]),
            new(Languages.Russian, localizationMock["Language_Russian"]),
            new(Languages.QazaqCyrillic, localizationMock["Language_QazaqCyrillic"]),
            new(Languages.QazaqLatin, localizationMock["Language_QazaqLatin"])
        ];

        AvailableThemes = [
            new(Themes.System, localizationMock["Theme_System"]),
            new(Themes.Light, localizationMock["Theme_Light"]),
            new(Themes.Dark, localizationMock["Theme_Dark"])
        ];
    }   

    public async Task LoadSettingsAsync()
    {
        var settings = await _settingsService.GetSettingsAsync();
        CodeLength = settings.CodeLength;
        MaxAttempts = settings.MaxAttempts;

        SelectedTheme = Enum.Parse<Themes>(settings.Theme, true);
        SelectedLanguage = Enum.Parse<Languages>(settings.Language, true);

        _themeService.Apply(SelectedTheme);
    }

    private async Task SaveSettingsAsync()
    {
        _themeService.Apply(SelectedTheme);

        await _settingsService.SaveSettingsAsync(new AppSettingsDto(CodeLength, MaxAttempts, SelectedTheme.ToString(), SelectedLanguage.ToString()));

        await Shell.Current.GoToAsync("//HomePage");
    }


}
