using BullsAndCows.Core.Interfaces;
using BullsAndCows.Core.Models;

namespace BullsAndCows.Core.Game;

public class GameEngine
{
    private readonly ISecretCodeGenerator _secretCodeGenerator;
    private readonly GuessEvaluator _guessEvaluator;

    public GameEngine(ISecretCodeGenerator secretCodeGenerator, GuessEvaluator guessEvaluator)
    {
        _secretCodeGenerator = secretCodeGenerator;
        _guessEvaluator = guessEvaluator;
    }

    public GameSession StartNewGame(GameSettings settings)
    {
        var secretCode = _secretCodeGenerator.Generate();

        return new GameSession(secretCode, settings, _guessEvaluator);
    }

}
