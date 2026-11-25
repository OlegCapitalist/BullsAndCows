using System.Globalization;
using System.Text;
using System.Text.Json;
using BullsAndCows.Application.Models;
using BullsAndCows.Application.Services;
using BullsAndCows.Infrastructure.Storage;
using BullsAndCows.UI.Interfaces;
using BullsAndCows.UI.Models;
using BullsAndCows.UI.Services;
using BullsAndCows.UI.ViewModels;
using FluentAssertions;
using Moq;
using System.IO.Abstractions.TestingHelpers;

namespace BullsAndCows.UI.IntegrationTests;

// LocalizationService changes DefaultThreadCurrentCulture and
// DefaultThreadCurrentUICulture (process-wide defaults).
[CollectionDefinition("SettingsPageViewModelCulture", DisableParallelization = true)]
public sealed class SettingsPageViewModelCultureCollection
{
}

[Collection("SettingsPageViewModelCulture")]
public class SettingsPageViewModelIntegrationTests : IDisposable
{
    private const string DefaultJson = """
        {
          "userSettings": {
            "codeLength": 4,
            "maxAttempts": 10,
            "theme": "System",
            "language": "Russian"
          },
          "databaseSettings": {
            "provider": "SQLite",
            "connectionString": "bullsandcows.db3"
          }
        }
        """;

    private readonly MockFileSystem _fileSystem;
    private readonly string _filePath;
    private readonly JsonSettingsStorage _storage;
    private readonly SettingsService _settingsService;
    private readonly LocalizationService _localizationService;
    private readonly Mock<IThemeService> _themeServiceMock;
    private readonly SettingsPageViewModel _sut;

    private readonly CultureInfo _originalCulture;
    private readonly CultureInfo _originalUICulture;
    private readonly CultureInfo? _originalDefaultCulture;
    private readonly CultureInfo? _originalDefaultUICulture;

    private int _defaultFactoryCalls;

    public SettingsPageViewModelIntegrationTests()
    {
        _originalCulture = CultureInfo.CurrentCulture;
        _originalUICulture = CultureInfo.CurrentUICulture;
        _originalDefaultCulture = CultureInfo.DefaultThreadCurrentCulture;
        _originalDefaultUICulture = CultureInfo.DefaultThreadCurrentUICulture;

        _fileSystem = new MockFileSystem();
        _filePath = Path.Combine(
            Path.GetTempPath(),
            "BullsAndCows.IntegrationTests",
            Guid.NewGuid().ToString("N"),
            "appsettings.json");

        _storage = CreateStorage();
        _settingsService = new SettingsService(_storage);

        // Actual localization service. Make its initial state explicit.
        _localizationService = new LocalizationService();
        _localizationService.Apply(Languages.Russian);

        // Keep MAUI Application.Current / Shell outside these headless tests.
        _themeServiceMock = new Mock<IThemeService>();
        _themeServiceMock.SetupGet(x => x.CurrentTheme).Returns(Themes.System);

        _sut = new SettingsPageViewModel(
            _settingsService,
            _themeServiceMock.Object,
            _localizationService);
    }

    [Fact]
    public async Task LoadSettingsAsync_WhenFileMissing_ShouldCreateFileAndPopulateViewModel()
    {
        // Act
        await _sut.LoadSettingsAsync();

        // Assert: ViewModel received values through two real services.
        _sut.CodeLength.Should().Be(4);
        _sut.MaxAttempts.Should().Be(10);
        _sut.SelectedThemeItem.Should().NotBeNull();
        _sut.SelectedThemeItem!.Value.Should().Be(Themes.System);
        _sut.SelectedLanguageItem.Should().NotBeNull();
        _sut.SelectedLanguageItem!.Value.Should().Be(Languages.Russian);

        // Assert: JsonSettingsStorage actually created a virtual file.
        _fileSystem.File.Exists(_filePath).Should().BeTrue();
        _defaultFactoryCalls.Should().Be(1);

        using var json = JsonDocument.Parse(_fileSystem.File.ReadAllText(_filePath));
        json.RootElement.GetProperty("userSettings")
            .GetProperty("codeLength").GetInt32().Should().Be(4);
    }

    [Fact]
    public async Task LoadSettingsAsync_WhenFileExists_ShouldLoadValuesAndApplyRealLocalization()
    {
        // Arrange: stored values differ from the application's current state.
        PutFile(CreateJson(6, 20, "Light", "English"));

        // Act
        await _sut.LoadSettingsAsync();

        // Assert
        _sut.CodeLength.Should().Be(6);
        _sut.MaxAttempts.Should().Be(20);
        _sut.SelectedTheme.Should().Be(Themes.Light);
        _sut.SelectedLanguage.Should().Be(Languages.English);
        _localizationService.CurrentLanguage.Should().Be(Languages.English);
        CultureInfo.DefaultThreadCurrentUICulture!.Name.Should().Be("en");

        // The theme is the only intentionally mocked boundary.
        _themeServiceMock.Verify(x => x.Apply(Themes.Light), Times.Once);
        _defaultFactoryCalls.Should().Be(0);
    }

    [Fact]
    public async Task LoadSettingsAsync_WhenStorageContainsInvalidJson_ShouldPropagateException()
    {
        // Arrange
        PutFile("this is not valid JSON");

        // Act
        Func<Task> act = () => _sut.LoadSettingsAsync();

        // Assert: error travels JsonSettingsStorage -> SettingsService -> ViewModel.
        await act.Should().ThrowAsync<JsonException>();
        _themeServiceMock.Verify(x => x.Apply(It.IsAny<Themes>()), Times.Never);
        _defaultFactoryCalls.Should().Be(0);
    }

    [Fact]
    public async Task LoadSettingsAsync_WhenStoredThemeIsUnknown_ShouldPropagateException()
    {
        // Arrange
        PutFile(CreateJson(4, 10, "UnknownTheme", "Russian"));

        // Act
        Func<Task> act = () => _sut.LoadSettingsAsync();

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
        _themeServiceMock.Verify(x => x.Apply(It.IsAny<Themes>()), Times.Never);
    }

    [Fact]
    public async Task LoadSettingsAsync_WhenLoadedTwice_ShouldUseSettingsCache()
    {
        // Arrange
        await _sut.LoadSettingsAsync();
        _fileSystem.File.WriteAllText(_filePath, "broken JSON after first load");

        // Act: if either real service re-read the file, parsing would fail.
        await _sut.LoadSettingsAsync();

        // Assert
        _sut.CodeLength.Should().Be(4);
        _sut.MaxAttempts.Should().Be(10);
        _defaultFactoryCalls.Should().Be(1);
    }

    [Fact]
    public async Task LoadSettingsAsync_ShouldReadSettingsPersistedByRealSettingsService()
    {
        // Arrange: save through real service and real JSON storage.
        var expected = new UserSettings(7, 15, "Dark", "English");
        await _settingsService.SaveSettingsAsync(expected);

        // A fresh service/storage must read the saved data, not the original cache.
        var freshStorage = CreateStorage();
        var freshSettingsService = new SettingsService(freshStorage);
        var freshViewModel = new SettingsPageViewModel(
            freshSettingsService,
            _themeServiceMock.Object,
            _localizationService);

        // Act
        await freshViewModel.LoadSettingsAsync();

        // Assert
        freshViewModel.CodeLength.Should().Be(expected.CodeLength);
        freshViewModel.MaxAttempts.Should().Be(expected.MaxAttempts);
        freshViewModel.SelectedTheme.Should().Be(Themes.Dark);
        freshViewModel.SelectedLanguage.Should().Be(Languages.English);
        _localizationService.CurrentLanguage.Should().Be(Languages.English);
        _themeServiceMock.Verify(x => x.Apply(Themes.Dark), Times.Once);

        using var json = JsonDocument.Parse(_fileSystem.File.ReadAllText(_filePath));
        json.RootElement.GetProperty("userSettings")
            .GetProperty("maxAttempts").GetInt32().Should().Be(15);
    }

    private JsonSettingsStorage CreateStorage() => new(
        _filePath,
        _ =>
        {
            _defaultFactoryCalls++;
            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(DefaultJson));
            return Task.FromResult(stream);
        },
        _fileSystem);

    private void PutFile(string json)
    {
        var directory = _fileSystem.Path.GetDirectoryName(_filePath)!;
        _fileSystem.Directory.CreateDirectory(directory);
        _fileSystem.AddFile(_filePath, new MockFileData(json));
    }

    private static string CreateJson(
        int codeLength,
        int maxAttempts,
        string theme,
        string language) => JsonSerializer.Serialize(new
        {
            userSettings = new { codeLength, maxAttempts, theme, language },
            databaseSettings = new
            {
                provider = "SQLite",
                connectionString = "bullsandcows.db3"
            }
        });

    public void Dispose()
    {
        CultureInfo.CurrentCulture = _originalCulture;
        CultureInfo.CurrentUICulture = _originalUICulture;
        CultureInfo.DefaultThreadCurrentCulture = _originalDefaultCulture;
        CultureInfo.DefaultThreadCurrentUICulture = _originalDefaultUICulture;
    }
}
