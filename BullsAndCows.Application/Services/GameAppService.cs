using BullsAndCows.Application.DTOs;
using BullsAndCows.Application.Interfaces;
using BullsAndCows.Core.Game;
using BullsAndCows.Core.Models;
using BullsAndCows.Core.Interfaces;
using BullsAndCows.Core.Enums;

namespace BullsAndCows.Application.Services;

public class GameAppService : IGameAppService
{
    private readonly IGameEngine _gameEngine;
    private string _secretCode = string.Empty;

    private IGameSession _currentSession = null!;

    public string SecretCode {  get; private set; } = string.Empty;

    public GameAppService(IGameEngine gameEngine)
    {
        _gameEngine = gameEngine;
    }

    public GameStateDto StartGame(StartGameRequest request)
    {
        var settings = new GameSettings(request.CodeLength, request.MaxAttempts);

        _currentSession = _gameEngine.StartNewGame(settings);

        _secretCode = string.Concat(_currentSession.SecretCode.Value.Select(d => d.ToString()));
        SecretCode = "";

        return MapGameSessionToGameStateDto(_currentSession);
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

        SecretCode = _currentSession.Status is GameStatus.Lost ? _secretCode : string.Empty;

        return new GuessResultDto(
            _currentSession.AttemptsCount,
            request.Value,
            result.Bulls,
            result.Cows,
            MapGameStatusToGameStatusDto(_currentSession.Status)
        );
    }

    public GameStateDto GetState()
    {
        if (_currentSession is null)
            throw new InvalidOperationException("Game is not started.");

        return MapGameSessionToGameStateDto(_currentSession);
    }

    private static GameStateDto MapGameSessionToGameStateDto(IGameSession session)
    {
        return new GameStateDto(
            MapGameStatusToGameStatusDto(session.Status),
            session.SecretCode.Value.Length,
            session.AttemptsCount
        );
    }

    private static GameStatusDto MapGameStatusToGameStatusDto(GameStatus status)
    {
        return status switch
        {
            GameStatus.NotStarted => GameStatusDto.NotStarted,
            GameStatus.InProgress => GameStatusDto.InProgress,
            GameStatus.Won => GameStatusDto.Won,
            GameStatus.Lost => GameStatusDto.Lost,
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };
    }
}
