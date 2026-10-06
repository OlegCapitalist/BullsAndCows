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
        var connection = await _database.GetConnectionAsync(cancellationToken);

        if (record.Id == Guid.Empty)
            record.Id = Guid.NewGuid();

        await connection.InsertAsync(record);
    }


    public async Task<IReadOnlyList<GameRecord>> GetBestAsync(
        int codeLength,
        int limit,
        CancellationToken cancellationToken = default)
    {
        var connection = await _database.GetConnectionAsync(cancellationToken);

        return await connection.Table<GameRecord>()
            .Where(x => x.CodeLength == codeLength)
            .OrderBy(x => x.AttemptsCount)
            .ThenBy(x => x.PlayedAt)    
            .Take(limit)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<int>> GetAvailableCodeLengthsAsync(CancellationToken cancellationToken = default)
    {
        var connection = await _database.GetConnectionAsync(cancellationToken);
        return await connection.QueryScalarsAsync<int>(
            """
            SELECT DISTINCT CodeLength
            FROM GameRecord
            ORDER BY CodeLength
            """);
    }

    public async Task ClearAsync(CancellationToken cancellationToken = default)
    {
        var connection = await _database.GetConnectionAsync(cancellationToken);

        await connection.DeleteAllAsync<GameRecord>();
    }

}
