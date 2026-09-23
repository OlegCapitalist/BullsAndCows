namespace BullsAndCows.Application.Models;

public record struct UserSettings(
    int CodeLength,
    int MaxAttempts,
    string Theme,
    string Language
);
