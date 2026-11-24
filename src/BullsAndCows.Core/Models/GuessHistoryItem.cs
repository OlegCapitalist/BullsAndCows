namespace BullsAndCows.Core.Models;

public record GuessHistoryItem(int AttemptNumber, Guess Guess, GuessResult Result, DateTime CreatedAt);

