using System;
using System.Globalization;
using BullsAndCows.UI.Models;
using BullsAndCows.UI.Services;
using FluentAssertions;
using Xunit;

namespace BullsAndCows.UI.UnitTests.Services;

// CultureInfo.DefaultThreadCurrentCulture and DefaultThreadCurrentUICulture
// affect the process, so these tests should not run alongside other tests.
[CollectionDefinition("LocalizationServiceCulture", DisableParallelization = true)]
public sealed class LocalizationServiceCultureCollection
{
}

public class LocalizationServiceTests : IDisposable
{
    private readonly CultureInfo _originalCurrentCulture = CultureInfo.CurrentCulture;
    private readonly CultureInfo _originalCurrentUICulture = CultureInfo.CurrentUICulture;
    private readonly CultureInfo? _originalDefaultThreadCurrentCulture = CultureInfo.DefaultThreadCurrentCulture;
    private readonly CultureInfo? _originalDefaultThreadCurrentUICulture = CultureInfo.DefaultThreadCurrentUICulture;

    private readonly LocalizationService _sut = new();

    [Theory]
    [InlineData(Languages.Russian, "ru")]
    [InlineData(Languages.English, "en")]
    [InlineData(Languages.QazaqCyrillic, "kk")]
    [InlineData(Languages.QazaqLatin, "kk-KZ")]
    public void Apply_ShouldSetLanguageAndCultures(Languages language, string expectedCulture)
    {
        // Act
        _sut.Apply(language);

        // Assert
        _sut.CurrentLanguage.Should().Be(language);
        CultureInfo.CurrentCulture.Name.Should().Be(expectedCulture);
        CultureInfo.CurrentUICulture.Name.Should().Be(expectedCulture);
        CultureInfo.DefaultThreadCurrentCulture!.Name.Should().Be(expectedCulture);
        CultureInfo.DefaultThreadCurrentUICulture!.Name.Should().Be(expectedCulture);
    }

    [Fact]
    public void Apply_WhenCalledAgain_ShouldChangeCurrentLanguageAndCulture()
    {
        // Arrange
        _sut.Apply(Languages.English);

        // Act
        _sut.Apply(Languages.Russian);

        // Assert
        _sut.CurrentLanguage.Should().Be(Languages.Russian);
        CultureInfo.CurrentCulture.Name.Should().Be("ru");
        CultureInfo.CurrentUICulture.Name.Should().Be("ru");
    }

    [Fact]
    public void Apply_WhenLanguageIsUnknown_ShouldUseRussianCulture()
    {
        // Arrange
        var unknownLanguage = (Languages)(-1);

        // Act
        _sut.Apply(unknownLanguage);

        // Assert
        CultureInfo.CurrentCulture.Name.Should().Be("ru");
        CultureInfo.CurrentUICulture.Name.Should().Be("ru");
    }

    public void Dispose()
    {
        CultureInfo.DefaultThreadCurrentCulture = _originalDefaultThreadCurrentCulture;
        CultureInfo.DefaultThreadCurrentUICulture = _originalDefaultThreadCurrentUICulture;
        CultureInfo.CurrentCulture = _originalCurrentCulture;
        CultureInfo.CurrentUICulture = _originalCurrentUICulture;
    }
}
