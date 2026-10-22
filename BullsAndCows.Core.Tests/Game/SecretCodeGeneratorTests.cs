using FluentAssertions;
using BullsAndCows.Core.Game;

namespace BullsAndCows.Core.Tests.Game;

public class SecretCodeGeneratorTests
{
    private readonly SecretCodeGenerator _sut = new();

    public static IEnumerable<object[]> CodeLengths =>
        Enumerable.Range(1, 10)
            .Select(length => new object[] { length });

    [Theory]
    [MemberData(nameof(CodeLengths))]
    public void Generate_ShouldReturnCodeWithRequestedLength(int codeLength)
    {
        // Act
        var result = _sut.Generate(codeLength);

        // Assert
        result.Value.Should().HaveCount(codeLength);
    }

    [Theory]
    [MemberData(nameof(CodeLengths))]
    public void Generate_ShouldNotContainDuplicateDigits(int codeLength)
    {
        // Act
        var result = _sut.Generate(codeLength);

        // Assert
        result.Value.Should().OnlyHaveUniqueItems();
    }

    [Fact]
    public void Generate_ShouldContainDigitsFromZeroToNine()
    {
        // Arrange
        const int codeLength = 10;

        // Act
        var result = _sut.Generate(codeLength);

        // Assert
        result.Value.Should().OnlyContain(digit => digit >= 0 && digit <= 9);
    }
}
