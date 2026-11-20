using BullsAndCows.UI.Models;
using BullsAndCows.UI.Services;
using FluentAssertions;
using MauiControlsApplication = Microsoft.Maui.Controls.Application;

namespace BullsAndCows.UI.Tests.Services;

[CollectionDefinition("ThemeServiceApplication", DisableParallelization = true)]
public sealed class ThemeServiceApplicationCollection
{
}

[Collection("ThemeServiceApplication")]
public class ThemeServiceTests : IDisposable
{
    private readonly MauiControlsApplication? _originalApplication;
    private readonly ThemeService _sut;

    public ThemeServiceTests()
    {
        _originalApplication = MauiControlsApplication.Current;
        _sut = new ThemeService();
    }

    [Theory]
    [InlineData(Themes.Light, AppTheme.Light)]
    [InlineData(Themes.Dark, AppTheme.Dark)]
    [InlineData(Themes.System, AppTheme.Unspecified)]
    public void Apply_ShouldSetCurrentThemeAndUserAppTheme(
        Themes theme,
        AppTheme expectedAppTheme)
    {
        // Arrange
        var app = new MauiControlsApplication();

        // Act
        _sut.Apply(theme);

        // Assert
        _sut.CurrentTheme.Should().Be(theme);
        app.UserAppTheme.Should().Be(expectedAppTheme);
    }

    [Fact]
    public void Apply_WhenCalledAgain_ShouldChangeTheme()
    {
        // Arrange
        var app = new MauiControlsApplication();
        _sut.Apply(Themes.Light);

        // Act
        _sut.Apply(Themes.Dark);

        // Assert
        _sut.CurrentTheme.Should().Be(Themes.Dark);
        app.UserAppTheme.Should().Be(AppTheme.Dark);
    }

    [Fact]
    public void Apply_WhenApplicationIsNull_ShouldNotChangeCurrentTheme()
    {
        // Arrange
        MauiControlsApplication.Current = null;
        var initialTheme = _sut.CurrentTheme;

        // Act
        _sut.Apply(Themes.Dark);

        // Assert
        _sut.CurrentTheme.Should().Be(initialTheme);
    }

    [Fact]
    public void Apply_WhenThemeIsUnknown_ShouldUseUnspecifiedAppTheme()
    {
        // Arrange
        var app = new MauiControlsApplication();
        var unknownTheme = (Themes)(-1);
        _sut.Apply(Themes.Dark);

        // Act
        _sut.Apply(unknownTheme);

        // Assert
        _sut.CurrentTheme.Should().Be(unknownTheme);
        app.UserAppTheme.Should().Be(AppTheme.Unspecified);
    }

    public void Dispose()
    {
        MauiControlsApplication.Current = _originalApplication;
    }
}
