using BullsAndCows.Application.DTOs;
using System.Threading;
using System.Threading.Tasks;

namespace BullsAndCows.Application.Interfaces;

public interface ISettingsStorage
{
    Task<AppSettingsDto> LoadAsync(CancellationToken cancellationToken = default);

    Task SaveAsync(AppSettingsDto settings, CancellationToken cancellationToken = default);
}
