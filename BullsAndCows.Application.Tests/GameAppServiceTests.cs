using BullsAndCows.Application.DTOs;
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

    public static TheoryData<GameSettings> GameSettingsData => new
        (
            Enumerable.Range(1, 10)
                .SelectMany(codeLength =>
                    new int[] { 5, 10, 20 }
                        .Select(maxAttempts =>
                            new GameSettings(codeLength, maxAttempts)))
        );

    public GameAppServiceTests()
    {
        _engineMock = new Mock<IGameEngine>();
        _sut = new GameAppService(_engineMock.Object);
    }

    [Theory]
    [MemberData(nameof(GameSettingsData))]
    public void StartGame_ShouldCreateSessionWithCorrectSettings(
    GameSettings settings)
    {
        // Arrange
        var sessionMock = CreateSession(
            Enumerable.Range(0, settings.CodeLength).ToArray());

        _engineMock
            .Setup(x => x.StartNewGame(It.IsAny<GameSettings>()))
            .Returns(sessionMock.Object);

        var request = new StartGameRequest(
            settings.CodeLength,
            settings.MaxAttempts);

        var expected = new GameStateDto(
            Status: GameStatusDto.InProgress,
            CodeLength: settings.CodeLength,
            AttemptsCount: 0);

        // Act
        var actual = _sut.StartGame(request);

        // Assert
        actual.Should().BeEquivalentTo(expected);

        _engineMock.Verify(x => x.StartNewGame(
            It.Is<GameSettings>(s =>
                s.CodeLength == settings.CodeLength &&
                s.MaxAttempts == settings.MaxAttempts)),
            Times.Once);

        _sut.SecretCode.Should().BeEmpty();
    }

    [Theory]
    [InlineData("1245", 2, 1, GameStatus.InProgress, GameStatusDto.InProgress, "")]
    [InlineData("1234", 4, 0, GameStatus.Won, GameStatusDto.Won, "")]
    [InlineData("5678", 0, 0, GameStatus.Lost, GameStatusDto.Lost, "1234")]
    public void MakeGuess_ShouldReturnCorrectResultAndRevealSecretOnlyOnLoss(
        string guessValue,
        int bulls,
        int cows,
        GameStatus resultingStatus,
        GameStatusDto expectedStatus,
        string expectedRevealedSecret)
    {
        // Arrange
        var sessionMock = CreateSession([1, 2, 3, 4]);
        var currentStatus = GameStatus.InProgress;
        var attemptsCount = 0;

        sessionMock.SetupGet(x => x.Status)
            .Returns(() => currentStatus);
        sessionMock.SetupGet(x => x.AttemptsCount)
            .Returns(() => attemptsCount);

        sessionMock
            .Setup(x => x.MakeGuess(It.IsAny<Guess>()))
            .Callback<Guess>(_ =>
            {
                attemptsCount++;
                currentStatus = resultingStatus;
            })
            .Returns(new GuessResult(bulls, cows));

        _engineMock
            .Setup(x => x.StartNewGame(It.IsAny<GameSettings>()))
            .Returns(sessionMock.Object);

        _sut.StartGame(new StartGameRequest(4, resultingStatus == GameStatus.Lost ? 1 : 10));

        var request = new MakeGuessRequest(guessValue);
        var expected = new GuessResultDto(
            1,
            guessValue,
            bulls,
            cows,
            expectedStatus);
        var expectedDigits = guessValue.Select(ch => ch - '0').ToArray();

        // Act
        var actual = _sut.MakeGuess(request);

        // Assert
        actual.Should().BeEquivalentTo(expected);

        sessionMock.Verify(x => x.MakeGuess(
            It.Is<Guess>(guess => guess.Value.SequenceEqual(expectedDigits))),
            Times.Once);

        _sut.SecretCode.Should().Be(expectedRevealedSecret);
    }

    [Theory]
    [InlineData(GameStatus.InProgress, GameStatusDto.InProgress, 0)]
    [InlineData(GameStatus.Won, GameStatusDto.Won, 3)]
    [InlineData(GameStatus.Lost, GameStatusDto.Lost, 10)]
    public void GetState_ShouldReturnCurrentSessionState(
        GameStatus status,
        GameStatusDto expectedStatus,
        int attemptsCount)
    {
        // Arrange
        var sessionMock = CreateSession([1, 2, 3, 4]);

        _engineMock
            .Setup(x => x.StartNewGame(It.IsAny<GameSettings>()))
            .Returns(sessionMock.Object);

        _sut.StartGame(new StartGameRequest(4, 10));

        // Имитируем изменение состояния сессии после начала игры.
        sessionMock.SetupGet(x => x.Status).Returns(status);
        sessionMock.SetupGet(x => x.AttemptsCount).Returns(attemptsCount);

        var expected = new GameStateDto(
            Status: expectedStatus,
            CodeLength: 4,
            AttemptsCount: attemptsCount);

        // Act
        var actual = _sut.GetState();

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void MakeGuess_ShouldThrowWhenGameIsNotStarted()
    {
        // Act
        Action act = () => _sut.MakeGuess(new MakeGuessRequest("1234"));

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Game is not started.");
    }

    [Fact]
    public void GetState_ShouldThrowWhenGameIsNotStarted()
    {
        // Act
        Action act = () => _sut.GetState();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Game is not started.");
    }

    [Fact]
    public void StartGame_ShouldHideSecretFromPreviousLostGame()
    {
        // Arrange
        var previousSession = CreateSession([1, 2, 3, 4]);
        var newSession = CreateSession([4, 3, 2, 1]);

        _engineMock
            .SetupSequence(x => x.StartNewGame(It.IsAny<GameSettings>()))
            .Returns(previousSession.Object)
            .Returns(newSession.Object);

        var previousStatus = GameStatus.InProgress;
        var previousAttempts = 0;
        previousSession.SetupGet(x => x.Status).Returns(() => previousStatus);
        previousSession.SetupGet(x => x.AttemptsCount).Returns(() => previousAttempts);
        previousSession.Setup(x => x.MakeGuess(It.IsAny<Guess>()))
            .Callback<Guess>(_ =>
            {
                previousAttempts++;
                previousStatus = GameStatus.Lost;
            })
            .Returns(new GuessResult(0, 0));

        _sut.StartGame(new StartGameRequest(4, 1));
        _sut.MakeGuess(new MakeGuessRequest("5678"));

        _sut.SecretCode.Should().Be("1234");

        // Act
        _sut.StartGame(new StartGameRequest(4, 10));

        // Assert
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
