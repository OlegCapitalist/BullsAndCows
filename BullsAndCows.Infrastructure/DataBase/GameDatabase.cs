
using BullsAndCows.Application.Models;
using SQLite;

namespace BullsAndCows.Infrastructure.DataBase;

public class GameDatabase
{
    private readonly SQLiteAsyncConnection _connection = null!;
    private bool _initialized;

    public GameDatabase(string databasePath)
    {
        _connection = new SQLiteAsyncConnection(databasePath);
    }

    public async Task InitializeAsync()
    {
        if (_initialized)
            return;

        await _connection.CreateTableAsync<GameRecord>();

        _initialized = true;
    }
    public async Task<SQLiteAsyncConnection> GetConnectionAsync()
    {
        await InitializeAsync();

        return _connection;
    }


}
