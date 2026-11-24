using BullsAndCows.UI.Models;

namespace BullsAndCows.UI.Interfaces;

public interface IThemeService
{
    Themes CurrentTheme { get; }

    void Apply(Themes theme);
}
