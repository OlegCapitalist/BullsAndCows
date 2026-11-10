using BullsAndCows.Application.Models;
using BullsAndCows.Infrastructure.Storage;
using FluentAssertions;
using System.IO.Abstractions.TestingHelpers;
using System.Text;
using System.Text.Json;

namespace BullsAndCows.Infrastructure.Tests;

public class JsonSettingsStorageTests
{
    private const string DefaultJson = """
        {
          "userSettings": {
            "codeLength": 4,
            "maxAttempts": 10,
            "theme": "System",
            "language": "Russian"
          },
          "databaseSettings": {
            "provider": "SQLite",
            "connectionString": "Data Source=bullsandcows.db3"
          }
        }
        """;

    private readonly MockFileSystem _fileSystem;
    private readonly string _filePath;
    private readonly JsonSettingsStorage _sut;
    private int _factoryCalls;

    public JsonSettingsStorageTests()
    {
        _fileSystem = new MockFileSystem();
        _filePath = Path.Combine(
            Path.GetTempPath(),
            "BullsAndCows.Infrastructure.Tests",
            Guid.NewGuid().ToString("N"),
            "Settings",
            "appsettings.json");

        _sut = new JsonSettingsStorage(
            _filePath,
            token =>
            {
                _factoryCalls++;
                return CreateDefaultStreamAsync(token);
            },
            _fileSystem);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_ShouldRejectInvalidPath(string? filePath)
    {
        // Act
        Action act = () => new JsonSettingsStorage(
            filePath!,
            CreateDefaultStreamAsync,
            _fileSystem);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public async Task LoadUserSettingsAsync_WhenFileDoesNotExist_ShouldCreateFileFromDefaults()
    {
        // Arrange
        var expected = new UserSettings(4, 10, "System", "Russian");

        // Act
        var actual = await _sut.LoadUserSettingsAsync();

        // Assert
        actual.Should().BeEquivalentTo(expected);
        _fileSystem.File.Exists(_filePath).Should().BeTrue();
        _fileSystem.File.ReadAllText(_filePath).Should().Be(DefaultJson);
        _factoryCalls.Should().Be(1);
    }

    [Fact]
    public async Task LoadDatabaseSettingsAsync_WhenFileExists_ShouldNotCallDefaultFactory()
    {
        // Arrange
        PutFile(DefaultJson);

        // Act
        var actual = await _sut.LoadDatabaseSettingsAsync();

        // Assert
        actual.Provider.Should().Be("SQLite");
        actual.ConnectionString.Should().Be("Data Source=bullsandcows.db3");
        _factoryCalls.Should().Be(0);
    }

    [Fact]
    public async Task LoadSettingsAsync_ShouldReuseCacheForBothSettingsTypes()
    {
        // Arrange
        var first = await _sut.LoadUserSettingsAsync();
        _fileSystem.File.WriteAllText(_filePath, "invalid JSON");

        // Act
        var second = await _sut.LoadUserSettingsAsync();
        var database = await _sut.LoadDatabaseSettingsAsync();

        // Assert
        second.Should().BeEquivalentTo(first);
        database.Provider.Should().Be("SQLite");
        database.ConnectionString.Should().Be("Data Source=bullsandcows.db3");
        _factoryCalls.Should().Be(1);
    }

    [Fact]
    public async Task SaveUserSettingsAsync_ShouldPersistChangesAndPreserveDatabaseSettings()
    {
        // Arrange
        PutFile(DefaultJson);
        var updated = new UserSettings(6, 20, "Dark", "English");

        // Act
        await _sut.SaveUserSettingsAsync(updated);

        // Assert: read from a new instance to bypass _sut's cache
        var reader = new JsonSettingsStorage(
            _filePath,
            CreateDefaultStreamAsync,
            _fileSystem);

        var actualUser = await reader.LoadUserSettingsAsync();
        var actualDatabase = await reader.LoadDatabaseSettingsAsync();

        actualUser.Should().BeEquivalentTo(updated);
        actualDatabase.Provider.Should().Be("SQLite");
        actualDatabase.ConnectionString.Should().Be("Data Source=bullsandcows.db3");
        _factoryCalls.Should().Be(0);

        using var document = JsonDocument.Parse(_fileSystem.File.ReadAllText(_filePath));
        document.RootElement.GetProperty("userSettings")
            .GetProperty("codeLength").GetInt32().Should().Be(6);
        document.RootElement.GetProperty("databaseSettings")
            .GetProperty("provider").GetString().Should().Be("SQLite");
    }

    [Fact]
    public async Task SaveUserSettingsAsync_WhenFileDoesNotExist_ShouldCreateIt()
    {
        // Arrange
        var updated = new UserSettings(10, 5, "Light", "English");

        // Act
        await _sut.SaveUserSettingsAsync(updated);

        // Assert
        _fileSystem.File.Exists(_filePath).Should().BeTrue();
        _factoryCalls.Should().Be(1);

        var reader = new JsonSettingsStorage(
            _filePath,
            CreateDefaultStreamAsync,
            _fileSystem);

        (await reader.LoadUserSettingsAsync()).Should().BeEquivalentTo(updated);
        (await reader.LoadDatabaseSettingsAsync()).Provider.Should().Be("SQLite");
    }

    [Fact]
    public async Task SaveUserSettingsAsync_ShouldUpdateInMemoryCache()
    {
        // Arrange
        await _sut.LoadUserSettingsAsync();
        var updated = new UserSettings(7, 15, "Dark", "English");

        // Act
        await _sut.SaveUserSettingsAsync(updated);
        _fileSystem.File.WriteAllText(_filePath, "invalid JSON");
        var actual = await _sut.LoadUserSettingsAsync();

        // Assert
        actual.Should().BeEquivalentTo(updated);
        _factoryCalls.Should().Be(1);
    }

    [Fact]
    public async Task LoadUserSettingsAsync_WhenJsonIsMalformed_ShouldThrowJsonException()
    {
        // Arrange
        PutFile("not valid JSON");

        // Act
        Func<Task> act = () => _sut.LoadUserSettingsAsync();

        // Assert
        await act.Should().ThrowAsync<JsonException>();
        _factoryCalls.Should().Be(0);
    }

    [Fact]
    public async Task LoadUserSettingsAsync_WhenJsonIsNull_ShouldThrowInvalidDataException()
    {
        // Arrange
        PutFile("null");

        // Act
        Func<Task> act = () => _sut.LoadUserSettingsAsync();

        // Assert
        await act.Should().ThrowAsync<InvalidDataException>();
        _factoryCalls.Should().Be(0);
    }

    [Fact]
    public async Task LoadUserSettingsAsync_ShouldForwardCancellationTokenToFactory()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        var token = cts.Token;
        CancellationToken receivedToken = default;

        var storage = new JsonSettingsStorage(
            _filePath,
            passedToken =>
            {
                receivedToken = passedToken;
                return CreateDefaultStreamAsync(passedToken);
            },
            _fileSystem);

        // Act
        await storage.LoadUserSettingsAsync(token);

        // Assert
        receivedToken.Should().Be(token);
    }

    private void PutFile(string json)
    {
        var directory = _fileSystem.Path.GetDirectoryName(_filePath)!;
        _fileSystem.Directory.CreateDirectory(directory);
        _fileSystem.AddFile(_filePath, new MockFileData(json));
    }

    private static Task<Stream> CreateDefaultStreamAsync(CancellationToken _) =>
        Task.FromResult<Stream>(
            new MemoryStream(Encoding.UTF8.GetBytes(DefaultJson)));
}
