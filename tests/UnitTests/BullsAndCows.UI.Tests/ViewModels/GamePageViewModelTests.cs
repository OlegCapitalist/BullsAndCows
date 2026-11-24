using BullsAndCows.Application.DTOs;
using BullsAndCows.Application.Interfaces;
using BullsAndCows.Application.Models;
using BullsAndCows.UI.Interfaces;
using BullsAndCows.UI.Models;
using BullsAndCows.UI.ViewModels;
using FluentAssertions;
using Moq;
using Xunit;

namespace BullsAndCows.UI.UnitTests.ViewModels;

public class GamePageViewModelTests
{
    private readonly Mock<IGameAppService> _gameAppServiceMock;
    private readonly Mock<ISettingsService> _settingsServiceMock;
    private readonly Mock<IGameRecordRepository> _repositoryMock;
    private readonly Mock<ILocalizationService> _localizationServiceMock;
    private readonly GamePageViewModel _sut;

    public GamePageViewModelTests()
    {
        _gameAppServiceMock = new Mock<IGameAppService>();
        _settingsServiceMock = new Mock<ISettingsService>();
        _repositoryMock = new Mock<IGameRecordRepository>();
        _localizationServiceMock = new Mock<ILocalizationService>();

        SetupSettings(4, 10);

        _gameAppServiceMock
            .Setup(x => x.StartGame(It.IsAny<StartGameRequest>()))
            .Returns((StartGameRequest request) => new GameStateDto(
                Status: GameStatusDto.InProgress,
                AttemptsCount: 0,
                CodeLength: request.CodeLength));

        // Completed Task makes command-based save tests deterministic
        // without awaiting an async-void ICommand.Execute handler.
        _repositoryMock
            .Setup(x => x.AddAsync(
                It.IsAny<GameRecord>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _sut = new GamePageViewModel(
            _gameAppServiceMock.Object,
            _settingsServiceMock.Object,
            _repositoryMock.Object,
            _localizationServiceMock.Object);
    }

    [Theory]
    [InlineData(4, 10)]
    [InlineData(6, 20)]
    public async Task StartGameAsync_ShouldPassSettingsAndSetInProgressStatus(
        int codeLength, int maxAttempts)
    {
        // Arrange
        SetupSettings(codeLength, maxAttempts);

        // Act
        await _sut.StartGameAsync();

        // Assert
        _gameAppServiceMock.Verify(x => x.StartGame(
            It.Is<StartGameRequest>(request =>
                request.CodeLength == codeLength &&
                request.MaxAttempts == maxAttempts)), Times.Once);

        _settingsServiceMock.Verify(x => x.GetSettingsAsync(
            It.IsAny<CancellationToken>()), Times.Once);

        _sut.Status.Should().Be(GameStatusDto.InProgress);
        _sut.CanMakeGuess.Should().BeTrue();
        _sut.IsWon.Should().BeFalse();
        _sut.IsLost.Should().BeFalse();
    }

    [Fact]
    public async Task StartGameAsync_ShouldClearPreviousAttemptsAndSecretCode()
    {
        // Arrange
        _sut.Attempts.Add(new Attempt(
            1, "1234", 1, 2, new DateTime(2026, 1, 1)));
        _sut.SecretCode = "1234";
        _sut.Status = GameStatusDto.Lost;

        // Act
        await _sut.StartGameAsync();

        // Assert
        _sut.Attempts.Should().BeEmpty();
        _sut.SecretCode.Should().BeEmpty();
        _sut.Status.Should().Be(GameStatusDto.InProgress);
    }

    [Fact]
    public async Task StartGameAsync_WhenSettingsLoadFails_ShouldNotStartGame()
    {
        // Arrange
        _settingsServiceMock
            .Setup(x => x.GetSettingsAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Settings not available"));

        // Act
        Func<Task> act = () => _sut.StartGameAsync();

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
        _gameAppServiceMock.Verify(
            x => x.StartGame(It.IsAny<StartGameRequest>()), Times.Never);
    }

    [Fact]
    public async Task AddDigitCommand_ShouldAppendUntilCodeLengthReached()
    {
        // Arrange
        await _sut.StartGameAsync();

        // Act
        foreach (var digit in new[] { "1", "2", "3", "4", "5" })
            _sut.AddDigitCommand.Execute(digit);

        // Assert
        _sut.Guess.Should().Be("1234");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task AddDigitCommand_WhenDigitIsNullOrEmpty_ShouldNotChangeGuess(
        string? digit)
    {
        // Arrange
        await _sut.StartGameAsync();
        _sut.Guess = "12";

        // Act
        _sut.AddDigitCommand.Execute(digit);

        // Assert
        _sut.Guess.Should().Be("12");
    }

    [Fact]
    public async Task BackspaceCommand_ShouldDeleteLastCharacter()
    {
        // Arrange
        await _sut.StartGameAsync();
        _sut.Guess = "123";

        // Act
        _sut.BackspaceCommand.Execute(null);

        // Assert
        _sut.Guess.Should().Be("12");
    }

    [Fact]
    public void BackspaceCommand_WhenGuessIsEmpty_ShouldKeepItEmpty()
    {
        // Act
        _sut.BackspaceCommand.Execute(null);

        // Assert
        _sut.Guess.Should().BeEmpty();
    }

    [Fact]
    public async Task ClearCommand_ShouldClearGuess()
    {
        // Arrange
        await _sut.StartGameAsync();
        _sut.Guess = "123";

        // Act
        _sut.ClearCommand.Execute(null);

        // Assert
        _sut.Guess.Should().BeEmpty();
    }

    [Fact]
    public async Task CheckGuessCommand_WhenGuessIsEmpty_ShouldNotMakeGuess()
    {
        // Arrange
        await _sut.StartGameAsync();
        _sut.Guess = "";

        // Act
        _sut.CheckGuessCommand.Execute(null);

        // Assert
        _gameAppServiceMock.Verify(
            x => x.MakeGuess(It.IsAny<MakeGuessRequest>()), Times.Never);
        _sut.Attempts.Should().BeEmpty();
        _repositoryMock.Verify(x => x.AddAsync(
            It.IsAny<GameRecord>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [InlineData(GameStatusDto.NotStarted)]
    [InlineData(GameStatusDto.Won)]
    [InlineData(GameStatusDto.Lost)]
    public async Task CheckGuessCommand_WhenGameIsNotInProgress_ShouldIgnoreGuess(
        GameStatusDto status)
    {
        // Arrange
        await _sut.StartGameAsync();
        _sut.Status = status;
        _sut.Guess = "1234";

        // Act
        _sut.CheckGuessCommand.Execute(null);

        // Assert
        _gameAppServiceMock.Verify(
            x => x.MakeGuess(It.IsAny<MakeGuessRequest>()), Times.Never);
        _sut.Guess.Should().Be("1234");
        _sut.Attempts.Should().BeEmpty();
    }

    [Fact]
    public async Task CheckGuessCommand_WhenGameContinues_ShouldAddAttemptAndClearGuess()
    {
        // Arrange
        await _sut.StartGameAsync();
        SetupGuessResult("1234", bulls: 1, cows: 2, status: GameStatusDto.InProgress);
        _sut.Guess = "1234";

        // Act
        _sut.CheckGuessCommand.Execute(null);

        // Assert
        _gameAppServiceMock.Verify(x => x.MakeGuess(
            It.Is<MakeGuessRequest>(request => request.Value == "1234")),
            Times.Once);

        _sut.Attempts.Should().ContainSingle();
        _sut.AttemptsCount.Should().Be(1);
        _sut.Status.Should().Be(GameStatusDto.InProgress);
        _sut.Guess.Should().BeEmpty();
        _sut.SecretCode.Should().BeEmpty();

        _repositoryMock.Verify(x => x.AddAsync(
            It.IsAny<GameRecord>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CheckGuessCommand_WhenCalledTwice_ShouldIncrementAttemptsCount()
    {
        // Arrange
        await _sut.StartGameAsync();
        SetupGuessResult("1234", bulls: 0, cows: 1, status: GameStatusDto.InProgress);

        // Act
        _sut.Guess = "1234";
        _sut.CheckGuessCommand.Execute(null);
        _sut.Guess = "1234";
        _sut.CheckGuessCommand.Execute(null);

        // Assert
        _sut.Attempts.Should().HaveCount(2);
        _sut.AttemptsCount.Should().Be(2);
        _gameAppServiceMock.Verify(x => x.MakeGuess(
            It.IsAny<MakeGuessRequest>()), Times.Exactly(2));
    }

    [Fact]
    public async Task CheckGuessCommand_WhenLost_ShouldRevealSecretWithoutSavingRecord()
    {
        // Arrange
        await _sut.StartGameAsync();
        SetupGuessResult("5678", bulls: 0, cows: 0, status: GameStatusDto.Lost);
        _gameAppServiceMock.SetupGet(x => x.SecretCode).Returns("1234");
        _sut.Guess = "5678";

        // Act
        _sut.CheckGuessCommand.Execute(null);

        // Assert
        _sut.Status.Should().Be(GameStatusDto.Lost);
        _sut.IsLost.Should().BeTrue();
        _sut.CanMakeGuess.Should().BeFalse();
        _sut.SecretCode.Should().Be("1234");
        _sut.Guess.Should().BeEmpty();
        _repositoryMock.Verify(x => x.AddAsync(
            It.IsAny<GameRecord>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CheckGuessCommand_WhenWon_ShouldSaveRecordWithCorrectValues()
    {
        // Arrange
        await _sut.StartGameAsync();
        SetupGuessResult("1234", bulls: 4, cows: 0, status: GameStatusDto.Won);
        _sut.Guess = "1234";
        GameRecord? savedRecord = null;

        _repositoryMock
            .Setup(x => x.AddAsync(
                It.IsAny<GameRecord>(), It.IsAny<CancellationToken>()))
            .Callback<GameRecord, CancellationToken>((record, _) => savedRecord = record)
            .Returns(Task.CompletedTask);

        // Act
        _sut.CheckGuessCommand.Execute(null);

        // Assert
        _sut.Status.Should().Be(GameStatusDto.Won);
        _sut.IsWon.Should().BeTrue();
        _sut.CanMakeGuess.Should().BeFalse();
        _sut.Guess.Should().BeEmpty();
        _sut.SecretCode.Should().BeEmpty();

        savedRecord.Should().NotBeNull();
        savedRecord.Should().BeEquivalentTo(new
        {
            CodeLength = 4,
            AttemptsCount = 1
        });

        _repositoryMock.Verify(x => x.AddAsync(
            It.IsAny<GameRecord>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    // This test intentionally exposes a currently observable bug in StartGameAsync.
    // It will FAIL until GamePageViewModel also clears Guess and resets AttemptsCount.
    [Fact]
    public async Task StartGameAsync_WhenRestarted_ShouldResetGuessAndAttemptsCount()
    {
        // Arrange
        await _sut.StartGameAsync();
        SetupGuessResult("5678", bulls: 0, cows: 0, status: GameStatusDto.InProgress);
        _sut.Guess = "5678";
        _sut.CheckGuessCommand.Execute(null);
        _sut.Guess = "12";

        // Act
        await _sut.StartGameAsync();

        // Assert
        _sut.Attempts.Should().BeEmpty();
        _sut.AttemptsCount.Should().Be(0); // Current production code does not reset this.
        _sut.Guess.Should().BeEmpty();     // Current production code does not reset this.
    }

    private void SetupSettings(int codeLength, int maxAttempts)
    {
        _settingsServiceMock
            .Setup(x => x.GetSettingsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UserSettings(
                codeLength, maxAttempts, "System", "Russian"));
    }

    private void SetupGuessResult(
        string value,
        int bulls,
        int cows,
        GameStatusDto status)
    {
        _gameAppServiceMock
            .Setup(x => x.MakeGuess(It.IsAny<MakeGuessRequest>()))
            .Returns(new GuessResultDto(1, value, bulls, cows, status));
    }
}
