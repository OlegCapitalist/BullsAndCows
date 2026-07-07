using BullsAndCows.Core.Game;
using BullsAndCows.Core.Models;

namespace BullsAndCows.Core.Interfaces;

public interface IGameEngine
{
    IGameSession StartNewGame(GameSettings settings);
}
