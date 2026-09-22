using BullsAndCows.Application.Models;

namespace BullsAndCows.Application.DTOs;

public record AppSettingsDto(
    GameSettings GameSettings,
    UiSettings UiSettings,
    DatabaseSettings DatabaseSettings
)
{
    public AppSettingsDto(GameSettings GameSettings, UiSettings UiSettings)
        : this(GameSettings, UiSettings, new DatabaseSettings())
    {
    }
}