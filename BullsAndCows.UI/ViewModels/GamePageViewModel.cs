using BullsAndCows.Application.Interfaces;
using BullsAndCows.UI.Models;
using PropertyChanged;
using System.Collections.ObjectModel;
using System.Windows.Input;
using BullsAndCows.Application.DTOs;

namespace BullsAndCows.UI.ViewModels;

[AddINotifyPropertyChangedInterface]
public class GamePageViewModel 
{
    private readonly IGameAppService _gameAppService = null!;
    private readonly ISettingsService _settingsService = null!;

    public string SecretCode { get; set; } = "";
    public string Guess { get; set; } = "";

    public GameStatusDto Status { get; set; } = GameStatusDto.NotStarted;

    public bool CanMakeGuess => Status is GameStatusDto.InProgress;
    public bool IsWon => Status is GameStatusDto.Won;
    public bool IsLost => Status is GameStatusDto.Lost;

    public int AttemptsCount => Attempts.Count;

    public ObservableCollection<Attempt> Attempts { get; set; } = new();

    #region Commands

    public ICommand AddDigitCommand => new Command<string>(AddDigit);

    public ICommand BackspaceCommand => new Command(Backspace);

    public ICommand ClearCommand => new Command(Clear);

    public ICommand CheckGuessCommand => new Command(CheckGuess);

    #endregion

    public GamePageViewModel(IGameAppService gameAppService, ISettingsService settingsService)
    {
        _gameAppService = gameAppService;
        _settingsService = settingsService;
    }

    public async Task StartGameAsync()
    {
        var settings = await _settingsService.GetSettingsAsync();

        var result = _gameAppService.StartGame(
            new StartGameRequest(
                settings.CodeLength,
                settings.MaxAttempts));

        SecretCode = "";
        Attempts.Clear();

        Status = result.Status;
    }

    private void AddDigit(string num)
    {
        if (string.IsNullOrEmpty(num) || Guess.Length >= 4)
            return;

        Guess += num;
    }

    private void Backspace()
    {
        if (Guess.Length > 0)
        {
            Guess = Guess.Substring(0, Guess.Length - 1);
        }
    }

    private void Clear()
    {
        Guess = "";
    }

    private void CheckGuess()
    {
        if (string.IsNullOrEmpty(Guess))
            return;

        if (Status != GameStatusDto.InProgress)
            return;

        var result = _gameAppService.MakeGuess(new MakeGuessRequest(Guess));
        Attempts.Add(new Attempt(Attempts.Count + 1, Guess, result.Bulls, result.Cows, DateTime.Now));

        Status = result.Status;
        SecretCode = Status is GameStatusDto.Lost ? _gameAppService.SecretCode : "";

        Guess = "";
    }


}
