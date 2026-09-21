using BullsAndCows.Application.DTOs;
using BullsAndCows.Application.Interfaces;

namespace BullsAndCows.Application.Services;

public class SettingsService : ISettingsService
{
    private readonly ISettingsStorage _settingsStorage;
    private AppSettingsDto? _current;
    public AppSettingsDto Current => _current ?? throw new InvalidOperationException("Settings are not loaded.");

    public SettingsService(ISettingsStorage settingsStorage)
    {
        _settingsStorage = settingsStorage;
    }

    public async Task<AppSettingsDto> GetSettingsAsync(
        CancellationToken cancellationToken = default)
    {
        _current = await _settingsStorage.LoadAsync(cancellationToken);

        return _current;
    }

    public async Task SaveSettingsAsync(
        AppSettingsDto settings,
        CancellationToken cancellationToken = default)
    {
        Validate(settings);

        _current = settings;
        await _settingsStorage.SaveAsync(settings, cancellationToken);
    }

    private static void Validate(AppSettingsDto settings)
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
