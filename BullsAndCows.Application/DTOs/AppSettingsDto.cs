using BullsAndCows.Application.Models;

namespace BullsAndCows.Application.DTOs;

public record AppSettingsDto(
    UserSettings UserSettings,
    DatabaseSettings DatabaseSettings
);
