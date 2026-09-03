using BullsAndCows.Core.Enums;

namespace BullsAndCows.Application.DTOs;

public record GuessResultDto(int AttemptNumber, string Guess, int Bulls, int Cows, GameStatus Status)
{
    public bool IsFinished => Status is GameStatus.Won
            or GameStatus.Lost
            or GameStatus.GaveUp;
}
