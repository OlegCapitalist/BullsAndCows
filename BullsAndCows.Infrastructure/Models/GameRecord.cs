using SQLite;

namespace BullsAndCows.Infrastructure.Models;

public class GameRecord
{
    [PrimaryKey]
    public Guid Id { get; set; }

    [Indexed("IX_GameRecord_CodeLength_AttemptsCount", 1)]
    public int CodeLength { get; set; }
    
    [Indexed("IX_GameRecord_CodeLength_AttemptsCount", 2)]
    public int AttemptsCount { get; set; }

    public DateTime PlayedAt { get; set; }
}
