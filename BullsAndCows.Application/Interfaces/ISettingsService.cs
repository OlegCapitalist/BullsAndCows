using BullsAndCows.Application.DTOs;
using BullsAndCows.Application.Models;
using System.Threading;
using System.Threading.Tasks;

namespace BullsAndCows.Application.Interfaces;

public interface ISettingsService
{
    AppSettingsDto Current { get; }

    Task<AppSettingsDto> GetSettingsAsync(CancellationToken cancellationToken = default);

    Task SaveSettingsAsync(GameSettings gameSettings, UiSettings uiSettings, CancellationToken cancellationToken = default);

}
