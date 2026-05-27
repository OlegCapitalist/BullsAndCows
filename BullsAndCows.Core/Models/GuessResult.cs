namespace BullsAndCows.Core.Models;

public record GuessResult(int Bulls, int Cows)
{
    public bool IsCorrect => Bulls == 4;
}
