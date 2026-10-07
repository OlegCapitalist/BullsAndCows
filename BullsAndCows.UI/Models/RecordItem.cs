namespace BullsAndCows.UI.Models;

public record RecordItem(
    int Number, 
    int AttemptsCount, 
    DateTime PlayedAt
)
{
    public bool IsEven => Number % 2 == 0;
}