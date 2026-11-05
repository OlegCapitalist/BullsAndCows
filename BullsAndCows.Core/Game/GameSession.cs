using BullsAndCows.Core.Enums;
using BullsAndCows.Core.Interfaces;
using BullsAndCows.Core.Models;

namespace BullsAndCows.Core.Game;

public class GameSession : IGameSession
{
    private readonly IGuessEvaluator _guessEvaluator;
    private readonly List<GuessHistoryItem> _history = [];
    private readonly GameSettings _settings;

    public SecretCode SecretCode { get; }

    public GameStatus Status { get; private set; }

    public IReadOnlyList<GuessHistoryItem> History => _history;

    public int AttemptsCount => _history.Count;

    public bool IsFinished => Status is GameStatus.Won
                                or GameStatus.Lost;

    public GameSession(
        SecretCode secretCode,
        GameSettings settings,
        IGuessEvaluator guessEvaluator)
    {
        SecretCode = secretCode;
        _guessEvaluator = guessEvaluator;
        _settings = settings;

        Status = GameStatus.InProgress;
    }

    public GuessResult MakeGuess(Guess guess)
    {
        var result = _guessEvaluator.Evaluate(SecretCode, guess);

        _history.Add(new GuessHistoryItem(
            _history.Count + 1,
            guess,
            result,
            DateTime.Now)
        );

        if (result.Bulls == SecretCode.Value.Length)
        {
            Status = GameStatus.Won;
        }
        else if (_history.Count >= _settings.MaxAttempts)
        {
            Status = GameStatus.Lost;
        }
        else
        {
            Status = GameStatus.InProgress;
        }

        return result;
    }
}
