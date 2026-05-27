using BullsAndCows.Core.Models;

namespace BullsAndCows.Core.Interfaces;

public interface ISecretCodeGenerator
{
    SecretCode Generate();
}
