using System.Collections.Generic;
using BullsAndCows.Core.Enums;
using BullsAndCows.Core.Models;

namespace BullsAndCows.Core.Interfaces;

public interface IGameSession
{
    SecretCode SecretCode { get; }

    GameStatus Status { get; }

    IReadOnlyList<GuessHistoryItem> History { get; }

    int AttemptsCount { get; }

    bool IsFinished { get; }

    GuessResult MakeGuess(Guess guess);

    void GiveUp();
}
