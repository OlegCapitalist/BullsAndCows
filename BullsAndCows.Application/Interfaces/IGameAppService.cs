using BullsAndCows.Application.DTOs;

namespace BullsAndCows.Application.Interfaces;

public interface IGameAppService
{
    GameStateDto StartGame(StartGameRequest request);

    GuessResultDto MakeGuess(MakeGuessRequest request);

    GameStateDto GiveUp();

    GameStateDto GetState();
}
