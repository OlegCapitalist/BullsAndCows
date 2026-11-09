using BullsAndCows.Application.DTOs;
using BullsAndCows.Application.Interfaces;
using BullsAndCows.Application.Models;
using System.IO.Abstractions;
using System.Text.Json;

namespace BullsAndCows.Infrastructure.Storage;

public class JsonSettingsStorage : ISettingsStorage
{
    private static readonly JsonSerializerOptions jsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly IFileSystem _fileSystem;
    private readonly string _filePath = String.Empty;
    private readonly Func<CancellationToken, Task<Stream>> _defaultSettingsStreamFactory;
    private AppSettingsDto? _current = null;

    public JsonSettingsStorage(string filePath,
        Func<CancellationToken, Task<Stream>> defaultSettingsStreamFactory,
        IFileSystem fileSystem)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        ArgumentNullException.ThrowIfNull(defaultSettingsStreamFactory);
        ArgumentNullException.ThrowIfNull(fileSystem);

        _filePath = filePath;
        _fileSystem = fileSystem;
        _defaultSettingsStreamFactory = defaultSettingsStreamFactory;
    }

    public async Task<UserSettings> LoadUserSettingsAsync(CancellationToken cancellationToken = default)
    {
        if (_current is not null)
            return _current.UserSettings;

        var appSettings = await LoadAsync(cancellationToken);
        return appSettings.UserSettings;
    }

    public async Task<DatabaseSettings> LoadDatabaseSettingsAsync(CancellationToken cancellationToken = default)
    {
        if (_current is not null)
            return _current.DatabaseSettings;

        var appSettings = await LoadAsync(cancellationToken);
        return appSettings.DatabaseSettings;
    }

    public async Task SaveUserSettingsAsync(
        UserSettings userSettings,
        CancellationToken cancellationToken = default)
    {
        if (_current is null)
            _current = await LoadAsync(cancellationToken);

        _current = _current with { UserSettings = userSettings };

        await SaveAsync(_current, cancellationToken);
    }

    private async Task<AppSettingsDto> LoadAsync(
        CancellationToken cancellationToken = default)
    {
        await EnsureCreatedAsync(cancellationToken);

        await using (var stream = _fileSystem.File.Open(
            _filePath,
            new FileStreamOptions
            {
                Mode = FileMode.Open,
                Access = FileAccess.Read,
                Share = FileShare.Read,
                BufferSize = 4096,
                Options = FileOptions.Asynchronous
            }))
        {
            _current = await JsonSerializer.DeserializeAsync<AppSettingsDto>(
                utf8Json: stream,
                options: jsonOptions,
                cancellationToken: cancellationToken)
                ?? throw new InvalidDataException(
                    "Settings file does not contain a valid object.");

            return _current;
        }
    }

    private async Task SaveAsync(
        AppSettingsDto settings,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(settings);

        var directory = Path.GetDirectoryName(_filePath);

        if (!string.IsNullOrWhiteSpace(directory))
            _fileSystem.Directory.CreateDirectory(directory);

        await using (var stream = _fileSystem.File.Open(
            _filePath,
            new FileStreamOptions
        {
            Mode = FileMode.Create,
            Access = FileAccess.Write,
            Share = FileShare.None,
            BufferSize = 4096,
            Options = FileOptions.Asynchronous
        }))
            {
                await JsonSerializer.SerializeAsync(
                    stream,
                    settings,
                    jsonOptions,
                    cancellationToken);
            }
        }

    private async Task EnsureCreatedAsync(
    CancellationToken cancellationToken = default)
    {
        if (_fileSystem.File.Exists(_filePath))
            return;

        var directory = _fileSystem.Path.GetDirectoryName(_filePath);

        if (!string.IsNullOrWhiteSpace(directory))
            _fileSystem.Directory.CreateDirectory(directory);

        await using var sourceStream =
            await _defaultSettingsStreamFactory(cancellationToken);

        await using var destinationStream = _fileSystem.File.Open(
            _filePath,
            new FileStreamOptions
            {
                Mode = FileMode.CreateNew,
                Access = FileAccess.Write,
                Share = FileShare.None,
                BufferSize = 4096,
                Options = FileOptions.Asynchronous
            });

        await sourceStream.CopyToAsync(
            destinationStream,
            cancellationToken);
    }
}
