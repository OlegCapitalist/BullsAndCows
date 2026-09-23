using BullsAndCows.Application.DTOs;
using BullsAndCows.Application.Models;
using System.Threading;
using System.Threading.Tasks;

namespace BullsAndCows.Application.Interfaces;

public interface ISettingsStorage
{
    Task<UserSettings> LoadUserSettingsAsync(CancellationToken cancellationToken = default);

    Task SaveUserSettingsAsync(UserSettings userSettings, CancellationToken cancellationToken = default);
}
