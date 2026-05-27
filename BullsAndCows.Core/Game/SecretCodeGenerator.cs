using BullsAndCows.Core.Models;
using System.Text;

namespace BullsAndCows.Core.Game;

public class SecretCodeGenerator
{
    private const int _codeLength = 4;
    public SecretCode GetCode()
    {
        var digits = Enumerable.Range(0, 9).Select(i => (byte)i).ToList();
        var result = new byte[_codeLength];

        for (int i = 0; i < _codeLength; i++)
        {
            var index = Random.Shared.Next(0, digits.Count);
            result[i] = (digits[index]);
            digits.RemoveAt(index);
        }

        return new SecretCode(result);
    }
}
