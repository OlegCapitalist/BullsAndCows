using BullsAndCows.Core.Enums;

namespace BullsAndCows.Application.DTOs;

public record GameStateDto(GameStatusDto Status, int CodeLength, int AttemptsCount)
{
    public bool IsStarted => Status != GameStatusDto.NotStarted;

    public bool IsInProgress => Status == GameStatusDto.InProgress;

    public bool IsFinished =>
        Status is GameStatusDto.Won
            or GameStatusDto.Lost;

    public bool IsWin => Status == GameStatusDto.Won;
}