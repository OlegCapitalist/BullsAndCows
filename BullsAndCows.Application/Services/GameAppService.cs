using BullsAndCows.Application.DTOs;
using BullsAndCows.Application.Interfaces;
using BullsAndCows.Core.Game;
using BullsAndCows.Core.Models;
using BullsAndCows.Core.Interfaces;

namespace BullsAndCows.Application.Services;

public class GameAppService : IGameAppService
{
    private readonly IGameEngine _gameEngine;

    private IGameSession _currentSession = null!;

    public GameAppService(IGameEngine gameEngine)
    {
        _gameEngine = gameEngine;
    }

    public GameStateDto StartGame(StartGameRequest request)
    {
        var settings = new GameSettings(
            codeLength: request.CodeLength,
            maxAttempts: request.MaxAttempts);

        _currentSession = _gameEngine.StartNewGame(settings);

        return MapToGameStateDto(_currentSession);
    }

    public GuessResultDto MakeGuess(MakeGuessRequest request)
    {
        if (_currentSession is null)
            throw new InvalidOperationException("Game is not started.");

        var digits = request.Value
            .Select(ch => int.Parse(ch.ToString()))
            .ToArray();

        var guess = new Guess(digits);

        var result = _currentSession.MakeGuess(guess);

        return new GuessResultDto(
            _currentSession.AttemptsCount,
            request.Value,
            result.Bulls,
            result.Cows,
            _currentSession.Status
        );
    }
    public GameStateDto GiveUp()
    {
        if (_currentSession is null)
            throw new InvalidOperationException("Game is not started.");

        _currentSession.GiveUp();

        return MapToGameStateDto(_currentSession);
    }

    public GameStateDto GetState()
    {
        if (_currentSession is null)
            throw new InvalidOperationException("Game is not started.");

        return MapToGameStateDto(_currentSession);
    }
    private static GameStateDto MapToGameStateDto(IGameSession session)
    {
        return new GameStateDto(
            session.Status,
            session.AttemptsCount,
            session.SecretCode.Value.Length
        );
    }
}
