using BullsAndCows.Application.Models;

namespace BullsAndCows.Application.Interfaces;

public interface IGameRecordRepository
{
    Task AddAsync(GameRecord record, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<GameRecord>> GetBestAsync(
        int codeLength,
        int limit,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<int>> GetAvailableCodeLengthsAsync(CancellationToken cancellationToken = default);
    Task ClearAsync(CancellationToken cancellationToken = default);
}
