using BullsAndCows.Application.DTOs;
using System.Threading;
using System.Threading.Tasks;

namespace BullsAndCows.Application.Interfaces;

public interface ISettingsService
{
    Task<AppSettingsDto> GetSettingsAsync(CancellationToken cancellationToken = default);

    Task SaveSettingsAsync(AppSettingsDto settings, CancellationToken cancellationToken = default);

    AppSettingsDto GetDefaultSettings();
}
