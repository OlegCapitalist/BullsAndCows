using BullsAndCows.Core.Interfaces;
using BullsAndCows.Core.Models;

namespace BullsAndCows.Core.Game;

public class GameEngine : IGameEngine
{
    private readonly ISecretCodeGenerator _secretCodeGenerator;
    private readonly IGuessEvaluator _guessEvaluator;

    public GameEngine(ISecretCodeGenerator secretCodeGenerator, IGuessEvaluator guessEvaluator)
    {
        _secretCodeGenerator = secretCodeGenerator;
        _guessEvaluator = guessEvaluator;
    }

    public IGameSession StartNewGame(GameSettings settings)
    {
        var secretCode = _secretCodeGenerator.Generate(settings.CodeLength);

        return new GameSession(secretCode, settings, _guessEvaluator);
    }

}
