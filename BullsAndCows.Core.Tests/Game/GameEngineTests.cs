using BullsAndCows.Core.Game;
using BullsAndCows.Core.Interfaces;
using BullsAndCows.Core.Models;
using FluentAssertions;
using Moq;

namespace BullsAndCows.Core.Tests.Game;

public class GameEngineTests
{
    private readonly Mock<ISecretCodeGenerator> _generatorMock;
    private readonly Mock<IGuessEvaluator> _evaluatorMock;
    private readonly GameEngine _sut;

    public GameEngineTests()
    {
        _generatorMock = new Mock<ISecretCodeGenerator>();
        _evaluatorMock = new Mock<IGuessEvaluator>();

        _sut = new GameEngine(
            _generatorMock.Object,
            _evaluatorMock.Object);
    }

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

        _generatorMock
            .Setup(x => x.Generate(settings.CodeLength))
            .Returns(secretCode);


        // Act
        _sut.StartNewGame(settings);

        // Assert
        _generatorMock.Verify(
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

        _generatorMock
            .Setup(x => x.Generate(settings.CodeLength))
            .Returns(secretCode);

        var expected = new GameSession(
            secretCode,
            settings,
            _evaluatorMock.Object);


        // Act
        var actual = _sut.StartNewGame(settings);

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }
}
