using BullsAndCows.Application.Models;

namespace BullsAndCows.Application.Interfaces;

public interface ISettingsService
{
    Task<UserSettings> GetSettingsAsync(CancellationToken cancellationToken = default);

    Task SaveSettingsAsync(UserSettings userSettings, CancellationToken cancellationToken = default);
}
