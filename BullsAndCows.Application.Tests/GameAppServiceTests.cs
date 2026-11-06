using BullsAndCows.Application.DTOs;
using BullsAndCows.Application.Interfaces;
using BullsAndCows.Application.Services;
using BullsAndCows.Core.Enums;
using BullsAndCows.Core.Interfaces;
using BullsAndCows.Core.Models;
using FluentAssertions;
using Moq;

namespace BullsAndCows.Application.Tests;

public class GameAppServiceTests
{
    private readonly Mock<IGameEngine> _engineMock;
    private readonly GameAppService _sut;

    public static IEnumerable<object[]> GameSettingsData =>
    Enumerable.Range(1, 10)
        .SelectMany(codeLength =>
            new[] { 5, 10, 20 }
                .Select(maxAttempts => new object[]
                {
                    codeLength,
                    maxAttempts
                }));

    public GameAppServiceTests()
    {
        _engineMock = new Mock<IGameEngine>();
        _sut = new GameAppService(_engineMock.Object);
    }

    [Theory]
    [MemberData(nameof(GameSettingsData))]
    public void StartGame_ShouldCreateSessionWithCorrectSettings(
    int codeLength,
    int maxAttempts)
    {
        // Arrange
        var sessionMock = CreateSession(
            Enumerable.Range(0, codeLength).ToArray());

        _engineMock
            .Setup(x => x.StartNewGame(It.IsAny<GameSettings>()))
            .Returns(sessionMock.Object);

        var request = new StartGameRequest(codeLength, maxAttempts);

        var expected = new GameStateDto(
            GameStatusDto.InProgress, 0, codeLength);

        // Act
        var actual = _sut.StartGame(request);

        // Assert
        actual.Should().BeEquivalentTo(expected);

        _engineMock.Verify(x => x.StartNewGame(
            It.Is<GameSettings>(s =>
                s.CodeLength == codeLength &&
                s.MaxAttempts == maxAttempts)),
            Times.Once);

        _sut.SecretCode.Should().BeEmpty();
    }

    private static Mock<IGameSession> CreateSession(
        int[] digits,
        GameStatus status = GameStatus.InProgress,
        int attemptsCount = 0)
    {
        var mock = new Mock<IGameSession>();

        mock.SetupGet(x => x.SecretCode)
            .Returns(new SecretCode(digits));

        mock.SetupGet(x => x.Status)
            .Returns(status);

        mock.SetupGet(x => x.AttemptsCount)
            .Returns(attemptsCount);

        return mock;
    }
}
