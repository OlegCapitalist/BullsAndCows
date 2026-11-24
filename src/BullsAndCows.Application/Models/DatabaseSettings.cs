namespace BullsAndCows.Application.Models;

public record struct DatabaseSettings(
    string Provider,
    string ConnectionString
);
