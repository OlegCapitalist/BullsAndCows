using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BullsAndCows.Application.Interfaces;
using BullsAndCows.Application.Models;
using BullsAndCows.UI.Interfaces;
using BullsAndCows.UI.Models;
using BullsAndCows.UI.ViewModels;
using FluentAssertions;
using Moq;
using Xunit;

namespace BullsAndCows.UI.Tests.ViewModels;

public class SettingsPageViewModelTests
{
    private readonly Mock<ISettingsService> _settingsServiceMock;
    private readonly Mock<IThemeService> _themeServiceMock;
    private readonly Mock<ILocalizationService> _localizationServiceMock;
    private readonly SettingsPageViewModel _sut;

    public SettingsPageViewModelTests()
    {
        _settingsServiceMock = new Mock<ISettingsService>();
        _themeServiceMock = new Mock<IThemeService>();
        _localizationServiceMock = new Mock<ILocalizationService>();

        // The ViewModel captures these values when its constructor runs.
        _themeServiceMock
            .SetupGet(x => x.CurrentTheme)
            .Returns(Themes.System);

        _localizationServiceMock
            .SetupGet(x => x.CurrentLanguage)
            .Returns(Languages.Russian);

        _sut = new SettingsPageViewModel(
            _settingsServiceMock.Object,
            _themeServiceMock.Object,
            _localizationServiceMock.Object);
    }

    [Theory]
    [InlineData(4, 10)]
    [InlineData(6, 20)]
    public async Task LoadSettingsAsync_ShouldPopulatePropertiesAndSelectedItems(
        int codeLength,
        int maxAttempts)
    {
        // Arrange
        SetupSettings(new UserSettings(
            codeLength, maxAttempts, "System", "Russian"));

        // Act
        await _sut.LoadSettingsAsync();

        // Assert
        _sut.CodeLength.Should().Be(codeLength);
        _sut.MaxAttempts.Should().Be(maxAttempts);
        _sut.SelectedTheme.Should().Be(Themes.System);
        _sut.SelectedLanguage.Should().Be(Languages.Russian);
        _sut.SelectedThemeItem.Should().NotBeNull();
        _sut.SelectedThemeItem!.Value.Should().Be(Themes.System);
        _sut.SelectedLanguageItem.Should().NotBeNull();
        _sut.SelectedLanguageItem!.Value.Should().Be(Languages.Russian);

        _sut.AvailableThemes.Select(x => x.Value)
            .Should().Equal(Enum.GetValues<Themes>());

        _sut.AvailableLanguages.Select(x => x.Value)
            .Should().Equal(
                Languages.English,
                Languages.Russian,
                Languages.QazaqCyrillic,
                Languages.QazaqLatin);

        _settingsServiceMock.Verify(
            x => x.GetSettingsAsync(It.IsAny<CancellationToken>()),
            Times.Once);

        _themeServiceMock.Verify(
            x => x.Apply(It.IsAny<Themes>()), Times.Never);
        _localizationServiceMock.Verify(
            x => x.Apply(It.IsAny<Languages>()), Times.Never);
    }

    [Fact]
    public async Task LoadSettingsAsync_WhenThemeAndLanguageDiffer_ShouldApplyBoth()
    {
        // Arrange
        SetupSettings(new UserSettings(4, 10, "Dark", "English"));

        // Act
        await _sut.LoadSettingsAsync();

        // Assert
        _sut.SelectedTheme.Should().Be(Themes.Dark);
        _sut.SelectedLanguage.Should().Be(Languages.English);
        _themeServiceMock.Verify(x => x.Apply(Themes.Dark), Times.Once);
        _localizationServiceMock.Verify(x => x.Apply(Languages.English), Times.Once);
    }

    [Fact]
    public async Task LoadSettingsAsync_WhenOnlyThemeDiffers_ShouldApplyOnlyTheme()
    {
        // Arrange
        SetupSettings(new UserSettings(4, 10, "Light", "Russian"));

        // Act
        await _sut.LoadSettingsAsync();

        // Assert
        _themeServiceMock.Verify(x => x.Apply(Themes.Light), Times.Once);
        _localizationServiceMock.Verify(
            x => x.Apply(It.IsAny<Languages>()), Times.Never);
    }

    [Fact]
    public async Task LoadSettingsAsync_WhenOnlyLanguageDiffers_ShouldApplyOnlyLanguage()
    {
        // Arrange
        SetupSettings(new UserSettings(4, 10, "System", "QazaqCyrillic"));

        // Act
        await _sut.LoadSettingsAsync();

        // Assert
        _localizationServiceMock.Verify(
            x => x.Apply(Languages.QazaqCyrillic), Times.Once);
        _themeServiceMock.Verify(
            x => x.Apply(It.IsAny<Themes>()), Times.Never);
    }

    [Fact]
    public async Task LoadSettingsAsync_WhenCalledTwice_ShouldNotApplySameThemeOrLanguageAgain()
    {
        // Arrange
        SetupSettings(new UserSettings(4, 10, "Dark", "English"));

        // Act
        await _sut.LoadSettingsAsync();
        await _sut.LoadSettingsAsync();

        // Assert
        _themeServiceMock.Verify(x => x.Apply(Themes.Dark), Times.Once);
        _localizationServiceMock.Verify(x => x.Apply(Languages.English), Times.Once);
        _settingsServiceMock.Verify(
            x => x.GetSettingsAsync(It.IsAny<CancellationToken>()),
            Times.Exactly(2));
    }

    [Theory]
    [InlineData(Themes.System)]
    [InlineData(Themes.Light)]
    [InlineData(Themes.Dark)]
    public void SelectedThemeItem_WhenAssigned_ShouldUpdateSelectedTheme(Themes theme)
    {
        // Act
        _sut.SelectedThemeItem = new SelectionItem<Themes>(theme, theme.ToString());

        // Assert
        _sut.SelectedTheme.Should().Be(theme);
    }

    [Theory]
    [InlineData(Languages.Russian)]
    [InlineData(Languages.English)]
    [InlineData(Languages.QazaqCyrillic)]
    [InlineData(Languages.QazaqLatin)]
    public void SelectedLanguageItem_WhenAssigned_ShouldUpdateSelectedLanguage(
        Languages language)
    {
        // Act
        _sut.SelectedLanguageItem =
            new SelectionItem<Languages>(language, language.ToString());

        // Assert
        _sut.SelectedLanguage.Should().Be(language);
    }

    [Fact]
    public void SelectedItems_WhenSetToNull_ShouldKeepSelectedValues()
    {
        // Arrange
        _sut.SelectedThemeItem = new SelectionItem<Themes>(Themes.Dark, "Dark");
        _sut.SelectedLanguageItem =
            new SelectionItem<Languages>(Languages.English, "English");

        // Act
        _sut.SelectedThemeItem = null;
        _sut.SelectedLanguageItem = null;

        // Assert
        _sut.SelectedTheme.Should().Be(Themes.Dark);
        _sut.SelectedLanguage.Should().Be(Languages.English);
    }

    [Theory]
    [InlineData("UnknownTheme", "Russian")]
    [InlineData("System", "UnknownLanguage")]
    public async Task LoadSettingsAsync_WhenEnumValueIsInvalid_ShouldThrow(
        string theme,
        string language)
    {
        // Arrange
        SetupSettings(new UserSettings(4, 10, theme, language));

        // Act
        Func<Task> act = () => _sut.LoadSettingsAsync();

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
        _themeServiceMock.Verify(
            x => x.Apply(It.IsAny<Themes>()), Times.Never);
        _localizationServiceMock.Verify(
            x => x.Apply(It.IsAny<Languages>()), Times.Never);
    }

    [Fact]
    public async Task LoadSettingsAsync_ShouldParseThemeAndLanguageIgnoringCase()
    {
        // Arrange
        SetupSettings(new UserSettings(4, 10, "dArK", "eNgLiSh"));

        // Act
        await _sut.LoadSettingsAsync();

        // Assert
        _sut.SelectedTheme.Should().Be(Themes.Dark);
        _sut.SelectedLanguage.Should().Be(Languages.English);
        _themeServiceMock.Verify(x => x.Apply(Themes.Dark), Times.Once);
        _localizationServiceMock.Verify(x => x.Apply(Languages.English), Times.Once);
    }

    [Fact]
    public async Task LoadSettingsAsync_WhenSettingsServiceFails_ShouldPropagateException()
    {
        // Arrange
        _settingsServiceMock
            .Setup(x => x.GetSettingsAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Test failure"));

        // Act
        Func<Task> act = () => _sut.LoadSettingsAsync();

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
        _themeServiceMock.Verify(
            x => x.Apply(It.IsAny<Themes>()), Times.Never);
        _localizationServiceMock.Verify(
            x => x.Apply(It.IsAny<Languages>()), Times.Never);
    }

    private void SetupSettings(UserSettings settings)
    {
        _settingsServiceMock
            .Setup(x => x.GetSettingsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(settings);
    }
}
