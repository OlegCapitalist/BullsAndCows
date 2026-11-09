using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using BullsAndCows.Application.Interfaces;
using BullsAndCows.Application.Models;
using BullsAndCows.Application.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace BullsAndCows.Application.Tests;

public class SettingsServiceTests
{
    private readonly Mock<ISettingsStorage> _storageMock;
    private readonly SettingsService _sut;

    public SettingsServiceTests()
    {
        _storageMock = new Mock<ISettingsStorage>();
        _sut = new SettingsService(_storageMock.Object);
    }

    [Fact]
    public async Task GetSettingsAsync_ShouldLoadSettingsOnceAndUseCache()
    {
        // Arrange
        var expected = CreateSettings();
        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        _storageMock
            .Setup(x => x.LoadUserSettingsAsync(token))
            .ReturnsAsync(expected);

        // Act
        var first = await _sut.GetSettingsAsync(token);
        var second = await _sut.GetSettingsAsync(token);

        // Assert
        first.Should().BeEquivalentTo(expected);
        second.Should().BeEquivalentTo(expected);
        _storageMock.Verify(x => x.LoadUserSettingsAsync(token), Times.Once);
    }

    [Fact]
    public async Task SaveSettingsAsync_ShouldSaveSettingsAndUpdateCache()
    {
        // Arrange
        var settings = CreateSettings();
        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        _storageMock
            .Setup(x => x.SaveUserSettingsAsync(settings, token))
            .Returns(Task.CompletedTask);

        // Act
        await _sut.SaveSettingsAsync(settings, token);
        var actual = await _sut.GetSettingsAsync();

        // Assert
        actual.Should().BeEquivalentTo(settings);
        _storageMock.Verify(x => x.SaveUserSettingsAsync(settings, token), Times.Once);
        _storageMock.Verify(
            x => x.LoadUserSettingsAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task SaveSettingsAsync_ShouldReplacePreviouslyLoadedSettings()
    {
        // Arrange
        var initial = CreateSettings(4, 10);
        var updated = CreateSettings(6, 20);

        _storageMock
            .Setup(x => x.LoadUserSettingsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(initial);
        _storageMock
            .Setup(x => x.SaveUserSettingsAsync(updated, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await _sut.GetSettingsAsync();

        // Act
        await _sut.SaveSettingsAsync(updated);
        var actual = await _sut.GetSettingsAsync();

        // Assert
        actual.Should().BeEquivalentTo(updated);
        _storageMock.Verify(
            x => x.LoadUserSettingsAsync(It.IsAny<CancellationToken>()),
            Times.Once);
        _storageMock.Verify(
            x => x.SaveUserSettingsAsync(updated, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(10, 1)]
    [InlineData(10, 20)]
    public async Task SaveSettingsAsync_ShouldAcceptValidBoundaryValues(
        int codeLength, int maxAttempts)
    {
        // Arrange
        var settings = CreateSettings(codeLength, maxAttempts);
        _storageMock
            .Setup(x => x.SaveUserSettingsAsync(settings, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _sut.SaveSettingsAsync(settings);

        // Assert
        _storageMock.Verify(
            x => x.SaveUserSettingsAsync(settings, It.IsAny<CancellationToken>()),
            Times.Once);
    }
    private static UserSettings CreateSettings(
        int codeLength = 4,
        int maxAttempts = 10) =>
        new(codeLength, maxAttempts, "System", "Russian");
}
