namespace BullsAndCows.Application.Models;

public record struct GameSettings(
    int CodeLength, 
    int MaxAttempts
);

