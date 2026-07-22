using BullsAndCows.UI.Models;
using PropertyChanged;

namespace BullsAndCows.UI.ViewModels;

[AddINotifyPropertyChangedInterface]
public class SettingsPageViewModel
{
    public int CodeLength { get; set; } = 4;

    public int MaxAttempts { get; set; } = 10;

    public int SelectedThemeIndex { get; set; } = 0;

    public int SelectedLanguageIndex { get; set; } = 0;

    public IReadOnlyList<Themes> AvailableThemes { get; } = Enum.GetValues<Themes>();

    public IReadOnlyList<Languages> AvailableLanguages { get; } = Enum.GetValues<Languages>();

    public Themes SelectedTheme { get; set; }

    public Languages SelectedLanguage { get; set; }

}
