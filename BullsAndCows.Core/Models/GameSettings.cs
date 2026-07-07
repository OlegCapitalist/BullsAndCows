using System;
using System.Collections.Generic;
using System.Text;

namespace BullsAndCows.Core.Models;

public class GameSettings
{
    public int CodeLength { get; }
    public int MaxAttempts { get; }

    public GameSettings(int codeLength, int maxAttempts)
    {
        CodeLength = codeLength;
        MaxAttempts = maxAttempts;
    }

    public static GameSettings Classic() => new(4, 10);
}
