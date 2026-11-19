using BullsAndCows.Application.Interfaces;
using BullsAndCows.Application.Models;
using BullsAndCows.Infrastructure.DataBase;
using FluentAssertions;
using Moq;

namespace BullsAndCows.Infrastructure.UnitTests;

public class GameDatabaseTests
{
    private readonly GameDatabase _sut;
    private readonly Mock<ISettingsStorage> _settingsStorageMock;

    public GameDatabaseTests()
    {
        _settingsStorageMock = new Mock<ISettingsStorage>();

        _settingsStorageMock
            .Setup(x => x.LoadDatabaseSettingsAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DatabaseSettings("SQLite", ":memory:"));

        _sut = new GameDatabase(_settingsStorageMock.Object, string.Empty);
    }

    [Fact]
    public async Task GetConnectionAsync_ShouldCreateGameRecordTable()
    {
        // Act
        var connection = await _sut.GetConnectionAsync();

        try
        {
            // Assert
            var tableInfo = await connection.GetTableInfoAsync("GameRecord");

            tableInfo.Should().NotBeEmpty();
        }
        finally
        {
            await connection.CloseAsync();
        }
    }

    [Fact]
    public async Task GetConnectionAsync_ShouldInitializeOnlyOnce()
    {
        // Act
        var first = await _sut.GetConnectionAsync();
        var second = await _sut.GetConnectionAsync();

        try
        {
            // Assert
            second.Should().BeSameAs(first);

            _settingsStorageMock.Verify(
                x => x.LoadDatabaseSettingsAsync(
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
        finally
        {
            await first.CloseAsync();
        }
    }

    [Theory]
    [InlineData("PostgreSQL")]
    [InlineData("SqlServer")]
    [InlineData("MySQL")]
    public async Task GetConnectionAsync_ShouldThrowForUnsupportedProvider(
    string provider)
    {
        // Arrange
        _settingsStorageMock
            .Setup(x => x.LoadDatabaseSettingsAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DatabaseSettings(provider, ":memory:"));

        // Act
        Func<Task> act = () => _sut.GetConnectionAsync();

        // Assert
        var exception = await act.Should()
            .ThrowAsync<NotSupportedException>();

        exception.Which.Message.Should().Contain(provider);
    }

    [Theory]
    [InlineData("SQLite")]
    [InlineData("sqlite")]
    [InlineData("SQLITE")]
    [InlineData("Sqlite")]
    public async Task GetConnectionAsync_ShouldAcceptSQLiteProviderRegardlessOfCase(
    string provider)
    {
        // Arrange
        _settingsStorageMock
            .Setup(x => x.LoadDatabaseSettingsAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DatabaseSettings(provider, ":memory:"));

        // Act
        var connection = await _sut.GetConnectionAsync();

        try
        {
            // Assert
            var tableInfo = await connection.GetTableInfoAsync("GameRecord");

            tableInfo.Should().NotBeEmpty();
        }
        finally
        {
            await connection.CloseAsync();
        }
    }

    [Fact]
    public async Task GetConnectionAsync_ShouldRetryAfterSettingsLoadFailure()
    {
        // Arrange
        _settingsStorageMock
            .SetupSequence(x => x.LoadDatabaseSettingsAsync(
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Settings load failed"))
            .ReturnsAsync(new DatabaseSettings("SQLite", ":memory:"));

        // Act
        Func<Task> firstAttempt = () => _sut.GetConnectionAsync();

        await firstAttempt.Should().ThrowAsync<InvalidOperationException>();

        var connection = await _sut.GetConnectionAsync();

        try
        {
            // Assert
            var tableInfo = await connection.GetTableInfoAsync("GameRecord");

            tableInfo.Should().NotBeEmpty();

            _settingsStorageMock.Verify(
                x => x.LoadDatabaseSettingsAsync(
                    It.IsAny<CancellationToken>()),
                Times.Exactly(2));
        }
        finally
        {
            await connection.CloseAsync();
        }
    }
}
