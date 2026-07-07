using BullsAndCows.Core.Enums;

namespace BullsAndCows.Application.DTOs;

public record GameStateDto(GameStatus Status, int CodeLength, int AttemptsCount)
{
    public bool IsFinished => Status != GameStatus.InProgress;
}
