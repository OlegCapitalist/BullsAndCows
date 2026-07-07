using BullsAndCows.Core.Interfaces;
using BullsAndCows.Core.Models;

namespace BullsAndCows.Core.Game;

public class SecretCodeGenerator : ISecretCodeGenerator
{
    private readonly int _codeLength = 4;

    public SecretCodeGenerator() { }

    public SecretCodeGenerator(int codeLength)
    {
        if (codeLength < 1 || codeLength > 10)
            throw new ArgumentOutOfRangeException(nameof(codeLength), "Code length must be between 1 and 10.");
        _codeLength = codeLength;
    }
    public SecretCode Generate()
    {
        var digits = Enumerable.Range(0, 10).Select(i => (byte)i).ToList();
        var result = new int[_codeLength];

        for (int i = 0; i < _codeLength; i++)
        {
            var index = Random.Shared.Next(0, digits.Count);
            result[i] = (digits[index]);
            digits.RemoveAt(index);
        }

        return new SecretCode(result);
    }
}
