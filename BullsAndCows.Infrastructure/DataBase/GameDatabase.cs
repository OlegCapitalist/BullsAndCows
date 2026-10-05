
using BullsAndCows.Application.Interfaces;
using BullsAndCows.Application.Models;
using SQLite;

namespace BullsAndCows.Infrastructure.DataBase;

public class GameDatabase
{
    private readonly ISettingsStorage _settingsStorage;
    private readonly string _appDataDirectory;

    private bool _initialized;
    private SQLiteAsyncConnection _connection = null!;

    public GameDatabase(
        ISettingsStorage settingsStorage,
        string appDataDirectory)
    {
        _settingsStorage = settingsStorage;
        _appDataDirectory = appDataDirectory;
    }
    public async Task<SQLiteAsyncConnection> GetConnectionAsync(CancellationToken cancellationToken = default)
    {
        await EnsureInitializedAsync(cancellationToken);

        return _connection
            ?? throw new InvalidOperationException(
                "Database is not initialized.");
    }

    private async Task EnsureInitializedAsync(CancellationToken cancellationToken = default)
    {
        if (_initialized)
            return;

        var databaseSettings =
            await _settingsStorage.LoadDatabaseSettingsAsync();

        if (!string.Equals(
                databaseSettings.Provider,
                "SQLite",
                StringComparison.OrdinalIgnoreCase))
        {
            throw new NotSupportedException(
                $"Database provider '{databaseSettings.Provider}' is not supported.");
        }

        var databasePath = Path.Combine(
            _appDataDirectory,
            databaseSettings.ConnectionString);

        _connection = new SQLiteAsyncConnection(databasePath);

        await _connection.CreateTableAsync<GameRecord>();

        _initialized = true;
    }
}
