namespace BullsAndCows.Application.DTOs;

public record AppSettingsDto(
    int CodeLength,
    int MaxAttempts,
    string theme,
    string language);