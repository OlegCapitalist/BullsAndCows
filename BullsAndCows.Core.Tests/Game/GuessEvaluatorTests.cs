using BullsAndCows.Core.Game;
using BullsAndCows.Core.Models;
using FluentAssertions;

namespace BullsAndCows.Core.Tests.Game;

public class GuessEvaluatorTests
{
    private readonly GuessEvaluator _sut = new();

    [Theory]
    [InlineData("1234", "1234", 4, 0)]
    [InlineData("1234", "4321", 0, 4)]
    [InlineData("1234", "1256", 2, 0)]
    [InlineData("1234", "5612", 0, 2)]
    [InlineData("1234", "1243", 2, 2)]
    [InlineData("1234", "5678", 0, 0)]
    [InlineData("1234", "1325", 1, 2)]
    public void Evaluate_ShouldReturnCorrectBullsAndCows(
    string secret,
    string attempt,
    int expectedBulls,
    int expectedCows)
    {
        // Arrange
        var secretCode = new SecretCode(secret.Select(c => c - '0').ToArray());
        var guess = new Guess(attempt.Select(c => c - '0').ToArray());

        // Act
        var result = _sut.Evaluate(secretCode, guess);

        // Assert
        result.Bulls.Should().Be(expectedBulls);
        result.Cows.Should().Be(expectedCows);
    }

    [Fact]
    public void Evaluate_ShouldThrow_WhenSecretCodeIsNull()
    {
        // Arrange
        var guess = new Guess([1, 2, 3, 4]);

        // Act
        Action act = () => _sut.Evaluate(null!, guess);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("secretCode");
    }

    [Fact]
    public void Evaluate_ShouldThrow_WhenGuessIsNull()
    {
        // Arrange
        var secretCode = new SecretCode([1, 2, 3, 4]);

        // Act
        Action act = () => _sut.Evaluate(secretCode, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("guess");
    }
}
