using BullsAndCows.Application.Interfaces;
using BullsAndCows.Application.Models;
using BullsAndCows.Infrastructure.DataBase;
using BullsAndCows.Infrastructure.Repositories;
using FluentAssertions;
using Moq;

namespace BullsAndCows.Infrastructure.Tests;

// SQLiteAsyncConnection pools connections by database path.
// The shared ':memory:' path must not be used by parallel tests.
[CollectionDefinition("SQLite in-memory", DisableParallelization = true)]
public sealed class SqliteMemoryCollectionDefinition { }

[Collection("SQLite in-memory")]
public class RepositoryTests
{
    private readonly GameDatabase _database;
    private readonly Repository _sut;

    public RepositoryTests()
    {
        var settingsStorageMock = new Mock<ISettingsStorage>();
        settingsStorageMock
            .Setup(x => x.LoadDatabaseSettingsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DatabaseSettings("SQLite", ":memory:"));

        _database = new GameDatabase(settingsStorageMock.Object, string.Empty);
        _sut = new Repository(_database);
    }

    [Fact]
    public async Task AddAsync_ShouldGenerateIdAndPersistRecord()
    {
        await RunWithFreshDatabaseAsync(async () =>
        {
            // Arrange
            var record = CreateRecord(4, 5, new DateTime(2026, 1, 1));

            // Act
            await _sut.AddAsync(record);
            var actual = await _sut.GetBestAsync(4, 10);

            // Assert
            record.Id.Should().NotBe(Guid.Empty);
            actual.Should().ContainSingle()
                .Which.Should().BeEquivalentTo(record);
        });
    }

    [Fact]
    public async Task GetBestAsync_ShouldFilterSortAndLimitRecords()
    {
        await RunWithFreshDatabaseAsync(async () =>
        {
            // Arrange
            var olderBest = CreateRecord(4, 3, new DateTime(2026, 1, 1));
            var newerBest = CreateRecord(4, 3, new DateTime(2026, 1, 2));
            var moreAttempts = CreateRecord(4, 7, new DateTime(2025, 1, 1));
            var differentLength = CreateRecord(6, 1, new DateTime(2025, 1, 1));

            // Deliberately insert in a different order than the expected sort order.
            await _sut.AddAsync(newerBest);
            await _sut.AddAsync(differentLength);
            await _sut.AddAsync(moreAttempts);
            await _sut.AddAsync(olderBest);

            // Act
            var actual = await _sut.GetBestAsync(codeLength: 4, limit: 2);

            // Assert
            actual.Should().BeEquivalentTo(
                new[] { olderBest, newerBest },
                options => options.WithStrictOrdering());
        });
    }

    [Fact]
    public async Task GetBestAsync_ShouldReturnEmptyWhenCodeLengthIsAbsent()
    {
        await RunWithFreshDatabaseAsync(async () =>
        {
            // Arrange
            await _sut.AddAsync(CreateRecord(4, 5, new DateTime(2026, 1, 1)));

            // Act
            var actual = await _sut.GetBestAsync(6, 10);

            // Assert
            actual.Should().BeEmpty();
        });
    }

    [Fact]
    public async Task GetBestAsync_ShouldReturnEmptyWhenLimitIsZero()
    {
        await RunWithFreshDatabaseAsync(async () =>
        {
            // Arrange
            await _sut.AddAsync(CreateRecord(4, 5, new DateTime(2026, 1, 1)));

            // Act
            var actual = await _sut.GetBestAsync(4, 0);

            // Assert
            actual.Should().BeEmpty();
        });
    }

    [Fact]
    public async Task GetAvailableCodeLengthsAsync_ShouldReturnDistinctSortedLengths()
    {
        await RunWithFreshDatabaseAsync(async () =>
        {
            // Arrange
            foreach (var codeLength in new[] { 6, 4, 6, 3, 4 })
            {
                await _sut.AddAsync(CreateRecord(
                    codeLength, 5, new DateTime(2026, 1, 1)));
            }

            // Act
            var actual = await _sut.GetAvailableCodeLengthsAsync();

            // Assert
            actual.Should().Equal(3, 4, 6);
        });
    }

    [Fact]
    public async Task GetAvailableCodeLengthsAsync_ShouldReturnEmptyForEmptyTable()
    {
        await RunWithFreshDatabaseAsync(async () =>
        {
            // Act
            var actual = await _sut.GetAvailableCodeLengthsAsync();

            // Assert
            actual.Should().BeEmpty();
        });
    }

    [Fact]
    public async Task ClearAsync_ShouldDeleteAllRecords()
    {
        await RunWithFreshDatabaseAsync(async () =>
        {
            // Arrange
            await _sut.AddAsync(CreateRecord(4, 5, new DateTime(2026, 1, 1)));
            await _sut.AddAsync(CreateRecord(6, 8, new DateTime(2026, 1, 2)));

            // Act
            await _sut.ClearAsync();

            // Assert
            (await _sut.GetBestAsync(4, 10)).Should().BeEmpty();
            (await _sut.GetBestAsync(6, 10)).Should().BeEmpty();
            (await _sut.GetAvailableCodeLengthsAsync()).Should().BeEmpty();
        });
    }

    [Fact]
    public async Task ClearAsync_ShouldSucceedWhenTableIsAlreadyEmpty()
    {
        await RunWithFreshDatabaseAsync(async () =>
        {
            // Act
            Func<Task> act = () => _sut.ClearAsync();

            // Assert
            await act.Should().NotThrowAsync();
            (await _sut.GetAvailableCodeLengthsAsync()).Should().BeEmpty();
        });
    }

    private async Task RunWithFreshDatabaseAsync(Func<Task> test)
    {
        var connection = await _database.GetConnectionAsync();
        try
        {
            // Safeguard against a previously pooled ':memory:' connection.
            await connection.DeleteAllAsync<GameRecord>();
            await test();
        }
        finally
        {
            await connection.CloseAsync();
        }
    }

    private static GameRecord CreateRecord(
        int codeLength,
        int attemptsCount,
        DateTime playedAt,
        Guid? id = null)
    {
        return new()
        {
            Id = id ?? Guid.Empty,
            CodeLength = codeLength,
            AttemptsCount = attemptsCount,
            PlayedAt = playedAt
        };
    }
}
