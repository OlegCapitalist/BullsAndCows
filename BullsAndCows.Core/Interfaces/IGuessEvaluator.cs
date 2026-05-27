using BullsAndCows.Core.Models;

namespace BullsAndCows.Core.Interfaces;

public interface IGuessEvaluator
{
    GuessResult Evaluate(SecretCode secretCode, Guess guess);
}
