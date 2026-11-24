using BullsAndCows.Application.DTOs;
using BullsAndCows.Application.Interfaces;
using BullsAndCows.Application.Models;
using BullsAndCows.UI.Models;
using BullsAndCows.UI.Interfaces;
using PropertyChanged;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace BullsAndCows.UI.ViewModels;

[AddINotifyPropertyChangedInterface]
public class GamePageViewModel 
{
    private readonly IGameAppService _gameAppService = null!;
    private readonly ISettingsService _settingsService = null!;
    private readonly IGameRecordRepository _repository = null!;
    private readonly ILocalizationService _localizationService = null!;
    private int _codeLength;

    public string SecretCode { get; set; } = "";
    public string Guess { get; set; } = "";

    public GameStatusDto Status { get; set; } = GameStatusDto.NotStarted;

    public bool CanMakeGuess => Status is GameStatusDto.InProgress;
    public bool IsWon => Status is GameStatusDto.Won;
    public bool IsLost => Status is GameStatusDto.Lost;

    public int AttemptsCount { get; private set; }

    public ObservableCollection<Attempt> Attempts { get; set; } = new();

    #region Commands

    public ICommand AddDigitCommand => new Command<string>(AddDigit);

    public ICommand BackspaceCommand => new Command(Backspace);

    public ICommand ClearCommand => new Command(Clear);

    public ICommand CheckGuessCommand => new Command(async () => await CheckGuessAsync());

    #endregion

    public GamePageViewModel(
        IGameAppService gameAppService, 
        ISettingsService settingsService, 
        IGameRecordRepository repository,
        ILocalizationService localizationService)
    {
        _gameAppService = gameAppService;
        _settingsService = settingsService;
        _repository = repository;
        _localizationService = localizationService;
    }

    public async Task StartGameAsync()
    {
        var settings = await _settingsService.GetSettingsAsync();

        _codeLength = settings.CodeLength;

        var result = _gameAppService.StartGame(
            new StartGameRequest(
                _codeLength,
                settings.MaxAttempts));
        
        SecretCode = "";
        Attempts.Clear();

        Guess = "";
        AttemptsCount = 0;

        Status = result.Status;
    }

    private void AddDigit(string num)
    {
        if (string.IsNullOrEmpty(num) || Guess.Length >= _codeLength)
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

    private async Task CheckGuessAsync()
    {
        if (string.IsNullOrEmpty(Guess))
            return;

        if (Status != GameStatusDto.InProgress)
            return;

        var result = _gameAppService.MakeGuess(new MakeGuessRequest(Guess));
        Attempts.Add(new Attempt(Attempts.Count + 1, Guess, result.Bulls, result.Cows, DateTime.Now));
        AttemptsCount = Attempts.Count;

        Status = result.Status;
        SecretCode = Status is GameStatusDto.Lost ? _gameAppService.SecretCode : "";

        if (Status is GameStatusDto.Won)
        {
            await SaveRecordsAsync();
        }

        Guess = "";
    }

    private async Task SaveRecordsAsync()
    {
        await _repository.AddAsync(
        new GameRecord
        {
            CodeLength = _codeLength,
            AttemptsCount = Attempts.Count,
            PlayedAt = DateTime.Now
        });
    }

}
