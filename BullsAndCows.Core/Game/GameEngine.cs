using BullsAndCows.Core.Interfaces;
using BullsAndCows.Core.Models;

namespace BullsAndCows.Core.Game;

public class GameEngine
{
    private readonly ISecretCodeGenerator _secretCodeGenerator;

    public GameEngine(ISecretCodeGenerator secretCodeGenerator)
    {
        _secretCodeGenerator = secretCodeGenerator;
    }

    public GameSession StartNewGame(GameSettings settings)
    {
        var secretCode = _secretCodeGenerator.Generate();

        return new GameSession(secretCode, settings);
    }

}
