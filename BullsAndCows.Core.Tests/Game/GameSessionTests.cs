using AutoFixture;
using BullsAndCows.Core.Enums;
using BullsAndCows.Core.Game;
using BullsAndCows.Core.Interfaces;
using BullsAndCows.Core.Models;
using FluentAssertions;
using Moq;

namespace BullsAndCows.Core.Tests.Game;

public class GameSessionTests
{
    private readonly Mock<IGuessEvaluator> _evaluatorMock;
    private readonly SecretCode _secretCode;
    private readonly GameSettings _settings;
    private readonly GameSession _sut;

    public GameSessionTests()
    {
        _evaluatorMock = new Mock<IGuessEvaluator>();
        _secretCode = new SecretCode([1, 2, 3, 4]);
        _settings = new GameSettings(4, 10);

        _sut = new GameSession(
            _secretCode,
            _settings,
            _evaluatorMock.Object);
    }

    [Fact]
    public void Constructor_ShouldInitializeGameSession()
    {
        // Assert
        _sut.SecretCode.Should().BeEquivalentTo(_secretCode);
        _sut.Status.Should().Be(GameStatus.InProgress);
        _sut.AttemptsCount.Should().Be(0);
        _sut.History.Should().BeEmpty();
        _sut.IsFinished.Should().BeFalse();
    }

    [Fact]
    public void MakeGuess_ShouldRemainInProgress_WhenGuessIsIncorrect()
    {
        // Arrange
        var guess = new Guess([5, 6, 7, 1]);
        var expected = new GuessResult(0, 1);

        _evaluatorMock
            .Setup(x => x.Evaluate(_secretCode, guess))
            .Returns(expected);

        // Act
        var actual = _sut.MakeGuess(guess);

        // Assert
        actual.Should().BeEquivalentTo(expected);
        _sut.Status.Should().Be(GameStatus.InProgress);
        _sut.AttemptsCount.Should().Be(1);
        _sut.History.Should().ContainSingle();
        _sut.IsFinished.Should().BeFalse();
    }

    [Fact]
    public void MakeGuess_ShouldSetWon_WhenAllBullsMatch()
    {
        // Arrange
        var guess = new Guess([1, 2, 3, 4]);

        _evaluatorMock
            .Setup(x => x.Evaluate(_secretCode, guess))
            .Returns(new GuessResult(4, 0));

        // Act
        _sut.MakeGuess(guess);

        // Assert
        _sut.Status.Should().Be(GameStatus.Won);
        _sut.IsFinished.Should().BeTrue();
    }

    [Fact]
    public void MakeGuess_ShouldSetLost_WhenMaxAttemptsReached()
    {
        // Arrange
        var settings = new GameSettings(4, 2);
        var sut = new GameSession(
            _secretCode,
            settings,
            _evaluatorMock.Object);

        var guess = new Guess([5, 6, 7, 8]);

        _evaluatorMock
            .Setup(x => x.Evaluate(_secretCode, guess))
            .Returns(new GuessResult(0, 0));

        // Act
        sut.MakeGuess(guess);
        sut.MakeGuess(guess);

        // Assert
        sut.Status.Should().Be(GameStatus.Lost);
        sut.AttemptsCount.Should().Be(2);
        sut.IsFinished.Should().BeTrue();
    }

    [Fact]
    public void GiveUp_ShouldSetGaveUp_WhenGameIsInProgress()
    {
        // Act
        _sut.GiveUp();

        // Assert
        _sut.Status.Should().Be(GameStatus.GaveUp);
        _sut.IsFinished.Should().BeTrue();
    }

}
