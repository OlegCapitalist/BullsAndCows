using BullsAndCows.Application.Interfaces;
using BullsAndCows.Application.Services;
using BullsAndCows.Core.Game;
using Microsoft.Extensions.DependencyInjection;
using BullsAndCows.Core.Interfaces;

namespace BullsAndCows.Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IGameEngine, GameEngine>();
        services.AddSingleton<ISecretCodeGenerator, SecretCodeGenerator>();
        services.AddSingleton<IGameAppService, GameAppService>();
        services.AddSingleton<IGuessEvaluator, GuessEvaluator>();

        return services;
    }
}
