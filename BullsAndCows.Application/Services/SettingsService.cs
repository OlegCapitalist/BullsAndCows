using BullsAndCows.Application.DTOs;
using BullsAndCows.Application.Interfaces;

namespace BullsAndCows.Application.Services;

public class SettingsService : ISettingsService
{
    private readonly ISettingsStorage _settingsStorage;

    public SettingsService(ISettingsStorage settingsStorage)
    {
        _settingsStorage = settingsStorage;
    }

    public async Task<AppSettingsDto> GetSettingsAsync(
        CancellationToken cancellationToken = default)
    {
        var settings = await _settingsStorage.LoadAsync(cancellationToken);

        return settings ?? GetDefaultSettings();
    }

    public async Task SaveSettingsAsync(
        AppSettingsDto settings,
        CancellationToken cancellationToken = default)
    {
        Validate(settings);

        await _settingsStorage.SaveAsync(settings, cancellationToken);
    }

    public AppSettingsDto GetDefaultSettings()
    {
        return new AppSettingsDto(4, 10);
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
