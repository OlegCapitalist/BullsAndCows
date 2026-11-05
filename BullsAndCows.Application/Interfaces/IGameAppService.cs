using BullsAndCows.Application.DTOs;

namespace BullsAndCows.Application.Interfaces;

public interface IGameAppService
{
    string SecretCode { get; }

    GameStateDto StartGame(StartGameRequest request);

    GuessResultDto MakeGuess(MakeGuessRequest request);

    GameStateDto GetState();
}
