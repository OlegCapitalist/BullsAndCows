using BullsAndCows.UI.Interfaces;
using BullsAndCows.UI.Models;
using System.Globalization;

namespace BullsAndCows.UI.Services
{
    public class LocalizationService : ILocalizationService
    {
        public Languages CurrentLanguage { get; private set; }

        public void Apply(Languages language)
        {
            var cultureName = language switch
            {
                Languages.Russian => "ru",
                Languages.English => "en",
                Languages.QazaqCyrillic => "kk-Cyrl",
                Languages.QazaqLatin => "kk-Latn",
                _ => "ru"
            };

            var culture = CultureInfo.GetCultureInfo(cultureName);

            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;

            CurrentLanguage = language;
        }
    }
}
