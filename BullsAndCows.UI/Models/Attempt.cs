using System;
using System.Collections.Generic;
using System.Text;

namespace BullsAndCows.UI.Models;

public record Attempt(int AttemptNumber, string Guess, int Bulls, int Cows, DateTime CreatedAt);
