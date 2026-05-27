using BullsAndCows.Core.Models;

namespace BullsAndCows.Core.Game;

public class GuessEvaluator
{
    public GuessResult Evaluate(SecretCode secretCode, Guess guess)
    {
        ArgumentNullException.ThrowIfNull(secretCode);
        ArgumentNullException.ThrowIfNull(guess);

        int bulls = guess.Value.Where((digit, index) => digit == secretCode.Value[index]).Count();
        int cows = guess.Value.Count(digit => secretCode.Value.Contains(digit)) - bulls;

        return new GuessResult(bulls, cows);
    }
}
