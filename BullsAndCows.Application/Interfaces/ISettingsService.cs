using BullsAndCows.Application.Models;

namespace BullsAndCows.Application.Interfaces;

public interface ISettingsService
{
    UserSettings Current { get; }

    Task<UserSettings> GetSettingsAsync(CancellationToken cancellationToken = default);

    Task SaveSettingsAsync(UserSettings userSettings, CancellationToken cancellationToken = default);
}
