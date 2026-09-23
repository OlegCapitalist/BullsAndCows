using BullsAndCows.Application.DTOs;
using BullsAndCows.Application.Interfaces;
using BullsAndCows.Application.Models;

namespace BullsAndCows.Application.Services;

public class SettingsService : ISettingsService
{
    private readonly ISettingsStorage _settingsStorage;
    private UserSettings? _current;
    public UserSettings Current => _current ?? throw new InvalidOperationException("Settings are not loaded.");

    public SettingsService(ISettingsStorage settingsStorage)
    {
        _settingsStorage = settingsStorage;
    }

    public async Task<UserSettings> GetSettingsAsync(
        CancellationToken cancellationToken = default)
    {
        _current = await _settingsStorage.LoadUserSettingsAsync(cancellationToken);

        return Current;
    }

    public async Task SaveSettingsAsync(
        UserSettings userSettings,
        CancellationToken cancellationToken = default)
    {
        Validate(userSettings);

        _current = userSettings;
        await _settingsStorage.SaveUserSettingsAsync(userSettings, cancellationToken);
    }

    private static void Validate(UserSettings settings)
    {
        if (settings.CodeLength is < 1 or > 10)
            throw new ArgumentOutOfRangeException(
                nameof(settings.CodeLength),
                "Code length must be between 1 and 10.");

        if (settings.MaxAttempts <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(settings.MaxAttempts),
                "Max attempts must be greater than zero.");
    }
}
