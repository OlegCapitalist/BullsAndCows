using BullsAndCows.Application.Interfaces;
using BullsAndCows.UI.Models;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using BullsAndCows.Application.DTOs;


namespace BullsAndCows.UI.ViewModels;

[AddINotifyPropertyChangedInterface]
public class GamePageViewModel 
{
    private IGameAppService _gameAppService = null!;
    public string Guess { get; set; } = "";

    public ObservableCollection<Attempt> Attempts { get; set; } = new();

    public ICommand AddDigitCommand => new Command<string>(AddDigit);

    public ICommand BackspaceCommand => new Command(Backspace);

    public ICommand ClearCommand => new Command(Clear);

    public ICommand CheckGuessCommand => new Command(CheckGuess);

    public GamePageViewModel(IGameAppService gameAppService)
    {
        _gameAppService = gameAppService;
        _gameAppService.StartGame(new StartGameRequest(4, 10));
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

        var result = _gameAppService.MakeGuess(new MakeGuessRequest(Guess));
        Attempts.Add(new Attempt(Attempts.Count + 1, Guess, result.Bulls, result.Cows, DateTime.Now));
        Guess = "";
    }
}
