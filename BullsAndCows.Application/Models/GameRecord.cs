namespace BullsAndCows.Application.Models;

public class GameRecord
{
    public Guid Id { get; set; }

    public int CodeLength { get; set; }

    public int AttemptsCount { get; set; }

    public DateTime PlayedAt { get; set; }
}
