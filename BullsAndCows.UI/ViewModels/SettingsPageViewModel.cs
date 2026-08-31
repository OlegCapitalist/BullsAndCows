using BullsAndCows.Application.DTOs;
using BullsAndCows.Application.Interfaces;
using BullsAndCows.Application.Services;
using BullsAndCows.UI.Models;
using PropertyChanged;
using System.Windows.Input;

namespace BullsAndCows.UI.ViewModels;

[AddINotifyPropertyChangedInterface]
public class SettingsPageViewModel
{
    private readonly ISettingsService _settingsService = null!;

    public int CodeLength { get; set; } = 4;

    public int MaxAttempts { get; set; } = 10;

    public int SelectedThemeIndex { get; set; } = 0;

    public int SelectedLanguageIndex { get; set; } = 0;

    public IReadOnlyList<Themes> AvailableThemes { get; } = Enum.GetValues<Themes>();

    public IReadOnlyList<Languages> AvailableLanguages { get; } = Enum.GetValues<Languages>();

    public Themes SelectedTheme { get; set; }

    public Languages SelectedLanguage { get; set; }

    public ICommand SaveSettingsCommand { get; }

    public SettingsPageViewModel(ISettingsService settingsService)
    {
        _settingsService = settingsService;

        SaveSettingsCommand = new Command(async () => await SaveSettingsAsync());
    }

    public async Task LoadSettingsAsync()
    {
        var settings = await _settingsService.GetSettingsAsync();
        CodeLength = settings.CodeLength;
        MaxAttempts = settings.MaxAttempts;

        SelectedTheme = Enum.Parse<Themes>(settings.Theme, true);
        SelectedLanguage = Enum.Parse<Languages>(settings.Language, true);
    }

    private async Task SaveSettingsAsync()
    {
        await _settingsService.SaveSettingsAsync(new AppSettingsDto(CodeLength, MaxAttempts, SelectedTheme.ToString(), SelectedLanguage.ToString()));

        await Shell.Current.GoToAsync("//HomePage");
    }


}
