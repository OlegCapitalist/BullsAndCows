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
        SecretCode = secretCode;
        _guessEvaluator = guessEvaluator;
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

        return result;
    }

    public void GiveUp()
    {
        if (Status != GameStatus.Started)
            return;

        Status = GameStatus.Finished;
    }

}
