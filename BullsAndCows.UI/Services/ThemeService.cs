using BullsAndCows.UI.Interfaces;
using BullsAndCows.UI.Models;

namespace BullsAndCows.UI.Services;

public class ThemeService : IThemeService
{
    public Themes CurrentTheme { get; private set; }

    public void Apply(Themes theme)
    {
        if (MauiApplication.Current is null)
            return;

        CurrentTheme = theme;

        MauiApplication.Current.UserAppTheme = theme switch
        {
            Themes.Light => AppTheme.Light,
            Themes.Dark => AppTheme.Dark,
            Themes.System => AppTheme.Unspecified,
            _ => AppTheme.Unspecified
        };
    }

}
