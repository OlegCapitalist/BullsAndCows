using BullsAndCows.Core.Enums;
using BullsAndCows.Core.Models;

namespace BullsAndCows.Core.Game;

public class GameSession
{
    private readonly GuessEvaluator _guessEvaluator = new();
    private readonly List<GuessHistoryItem> _history = [];

    public SecretCode SecretCode { get; }

    public GameStatus Status { get; private set; }

    public IReadOnlyList<GuessHistoryItem> History => _history;

    public int AttemptsCount => _history.Count;

    public bool IsFinished => Status != GameStatus.Finished;
    public GameSession(SecretCode secretCode)
    {
        
    }

    public GameSession(
        SecretCode secretCode,
        GameSettings settings,
        GuessEvaluator guessEvaluator)
    {

    }

    public GuessResult MakeGuess(Guess guess)
    {
        var result = _guessEvaluator.Evaluate(SecretCode, guess);

        var historyItem = new GuessHistoryItem(
            _history.Count + 1,
            guess,
            result,
            DateTime.UtcNow);

        _history.Add(historyItem);

        return result;
    }

    public void GiveUp()
    {
        if (Status != GameStatus.Started)
            return;

        Status = GameStatus.Finished;
    }

}
