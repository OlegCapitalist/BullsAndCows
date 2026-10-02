using BullsAndCows.Application.Interfaces;
using BullsAndCows.Application.Models;
using BullsAndCows.Infrastructure.DataBase;

namespace BullsAndCows.Infrastructure.Repositories;

public class Repository : IGameRecordRepository
{
    private readonly GameDatabase _database;

    public Repository(GameDatabase database)
    {
        _database = database;
    }

    public async Task AddAsync(GameRecord record, CancellationToken cancellationToken = default)
    {
        
    }


    public async Task<IReadOnlyList<GameRecord>> GetBestAsync(
        int codeLength,
        int limit,
        CancellationToken cancellationToken = default)
    {
        return new List<GameRecord>();
    }

    public async Task ClearAsync(CancellationToken cancellationToken = default)
    {
    }
}
