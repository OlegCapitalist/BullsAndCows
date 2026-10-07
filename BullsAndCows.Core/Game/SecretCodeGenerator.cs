using BullsAndCows.Core.Interfaces;
using BullsAndCows.Core.Models;

namespace BullsAndCows.Core.Game;

public class SecretCodeGenerator : ISecretCodeGenerator
{
    public SecretCode Generate(int codeLength)
    {
        var digits = Enumerable.Range(0, 10).Select(i => (byte)i).ToList();
        var result = new int[codeLength];

        for (int i = 0; i < codeLength; i++)
        {
            var index = Random.Shared.Next(0, digits.Count);
            result[i] = (digits[index]);
            digits.RemoveAt(index);
        }

        return new SecretCode(result);
    }
}
