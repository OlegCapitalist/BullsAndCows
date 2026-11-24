using BullsAndCows.Core.Enums;

namespace BullsAndCows.Application.DTOs;

public record GuessResultDto(int AttemptNumber, string Guess, int Bulls, int Cows, GameStatusDto Status)
{
    public bool IsWin => Status == GameStatusDto.Won;

    public bool IsFinished =>
        Status is GameStatusDto.Won
            or GameStatusDto.Lost;
}
