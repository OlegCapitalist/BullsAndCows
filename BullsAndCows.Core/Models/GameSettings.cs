namespace BullsAndCows.Core.Models;

//public class GameSettings
//{
//    public int CodeLength { get; }
//    public int MaxAttempts { get; }

//    public GameSettings(int codeLength, int maxAttempts)
//    {
//        CodeLength = codeLength;
//        MaxAttempts = maxAttempts;
//    }
//}

public record GameSettings(int CodeLength, int MaxAttempts);