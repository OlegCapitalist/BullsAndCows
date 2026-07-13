using BullsAndCows.Application.Interfaces;
using BullsAndCows.Application.Services;
using BullsAndCows.Core.Game;
using Microsoft.Extensions.DependencyInjection;

namespace BullsAndCows.Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<GameEngine>();

        services.AddSingleton<IGameAppService, GameAppService>();

        return services;
    }
}
