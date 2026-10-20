using BullsAndCows.UI.Interfaces;
using BullsAndCows.UI.Models;
using System.Globalization;
using BullsAndCows.UI.Resources.Localization;

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
                Languages.QazaqCyrillic => "kk",
                Languages.QazaqLatin => "kk-kz",
                _ => "ru"
            };

            var culture = CultureInfo.GetCultureInfo(cultureName);

            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;

            var test = AppResources.Theme_System;

            CurrentLanguage = language;
        }
    }
}
