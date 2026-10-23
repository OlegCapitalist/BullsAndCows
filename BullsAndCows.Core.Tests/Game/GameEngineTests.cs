using AutoFixture;
using BullsAndCows.Core.Game;
using BullsAndCows.Core.Interfaces;
using BullsAndCows.Core.Models;
using FluentAssertions;
using Moq;

namespace BullsAndCows.Core.Tests.Game;

public class GameEngineTests
{
    [Theory]
    [InlineData(1, 5)]
    [InlineData(4, 10)]
    [InlineData(6, 15)]
    [InlineData(10, 20)]
    public void StartNewGame_ShouldCallGeneratorWithCorrectCodeLength(int codeLength, int maxAttempts)
    {
        // Arrange
        var settings = new GameSettings(codeLength, maxAttempts);
        var secretCode = new SecretCode(Enumerable.Range(0, codeLength).ToArray());

        var generatorMock = new Mock<ISecretCodeGenerator>();
        var evaluatorMock = new Mock<IGuessEvaluator>();

        generatorMock
            .Setup(x => x.Generate(settings.CodeLength))
            .Returns(secretCode);

        var sut = new GameEngine(
            generatorMock.Object,
            evaluatorMock.Object);

        // Act
        sut.StartNewGame(settings);

        // Assert
        generatorMock.Verify(
            x => x.Generate(settings.CodeLength),
            Times.Once);
    }

    [Theory]
    [InlineData(1, 5)]
    [InlineData(4, 10)]
    [InlineData(6, 15)]
    [InlineData(10, 20)]
    public void StartNewGame_ShouldReturnCorrectGameSession(int codeLength, int maxAttempts)
    {
        // Arrange
        var secretCode = new SecretCode(Enumerable.Range(0, codeLength).ToArray());
        var settings = new GameSettings(codeLength, maxAttempts);

        var generatorMock = new Mock<ISecretCodeGenerator>();
        var evaluatorMock = new Mock<IGuessEvaluator>();

        generatorMock
            .Setup(x => x.Generate(settings.CodeLength))
            .Returns(secretCode);

        var expected = new GameSession(
            secretCode,
            settings,
            evaluatorMock.Object);

        var sut = new GameEngine(
            generatorMock.Object,
            evaluatorMock.Object);

        // Act
        var actual = sut.StartNewGame(settings);

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }
}
