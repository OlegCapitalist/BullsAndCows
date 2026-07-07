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

    public JsonSettingsStorage(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("Settings file path cannot be empty.", nameof(filePath));

        _filePath = filePath;
    }

    public async Task<AppSettingsDto?> LoadAsync(
        CancellationToken cancellationToken = default)
    {
        if (!File.Exists(_filePath))
            return null;

        await using var stream = File.OpenRead(_filePath);

        return await JsonSerializer.DeserializeAsync<AppSettingsDto>(
            stream,
            jsonOptions,
            cancellationToken);
    }

    public async Task SaveAsync(
        AppSettingsDto settings,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(settings);

        var directory = Path.GetDirectoryName(_filePath);

        if (!string.IsNullOrWhiteSpace(directory))
            Directory.CreateDirectory(directory);

        await using (var stream = File.Create(_filePath))
        {
            await JsonSerializer.SerializeAsync(
                        stream,
                        settings,
                        jsonOptions,
                        cancellationToken);
        }
    }

}
