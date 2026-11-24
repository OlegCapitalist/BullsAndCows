using BullsAndCows.UI.Models;

namespace BullsAndCows.UI.Interfaces;

public interface ILocalizationService
{
    Languages CurrentLanguage { get; }
    void Apply(Languages language);
}
