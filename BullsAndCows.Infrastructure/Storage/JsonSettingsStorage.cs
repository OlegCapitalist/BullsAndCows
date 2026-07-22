using System.Text.Json;
using BullsAndCows.Application.DTOs;
using BullsAndCows.Application.Interfaces;

namespace BullsAndCows.Infrastructure.Storage;

public class JsonSettingsStorage : ISettingsStorage
{
    private static readonly JsonSerializerOptions jsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly string _filePath = String.Empty;
    private readonly Func<CancellationToken, Task<Stream>> _defaultSettingsStreamFactory;

    public JsonSettingsStorage(string filePath,
        Func<CancellationToken, Task<Stream>> defaultSettingsStreamFactory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        ArgumentNullException.ThrowIfNull(defaultSettingsStreamFactory);

        _filePath = filePath;
        _defaultSettingsStreamFactory = defaultSettingsStreamFactory;
    }

    public async Task<AppSettingsDto> LoadAsync(
        CancellationToken cancellationToken = default)
    {
        await EnsureCreatedAsync(cancellationToken);

        await using (var stream = new FileStream(
            _filePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 4096,
            useAsync: true))
        {
            return await JsonSerializer.DeserializeAsync<AppSettingsDto>(
                stream,
                jsonOptions,
                cancellationToken)
                ?? throw new InvalidDataException(
                    "Settings file does not contain a valid object.");
        }
    }

    public async Task SaveAsync(
        AppSettingsDto settings,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(settings);

        var directory = Path.GetDirectoryName(_filePath);

        if (!string.IsNullOrWhiteSpace(directory))
            Directory.CreateDirectory(directory);

        await using (var stream = new FileStream(
            _filePath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 4096,
            useAsync: true))
        {
            await JsonSerializer.SerializeAsync(
                        stream,
                        settings,
                        jsonOptions,
                        cancellationToken);
        }
    }

    public async Task EnsureCreatedAsync(CancellationToken cancellationToken = default)
    {
        if (File.Exists(_filePath))
            return;

        var directory = Path.GetDirectoryName(_filePath);

        if (!string.IsNullOrWhiteSpace(directory))
            Directory.CreateDirectory(directory);

        await using var sourceStream =
            await _defaultSettingsStreamFactory(cancellationToken);

        await using var destinationStream = new FileStream(
            _filePath,
            FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 4096,
            useAsync: true);

        await sourceStream.CopyToAsync(
            destinationStream,
            cancellationToken);
    }
}
