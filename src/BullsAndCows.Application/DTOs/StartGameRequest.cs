namespace BullsAndCows.Application.DTOs;

public record StartGameRequest(int CodeLength = 4, int MaxAttempts = 10);

