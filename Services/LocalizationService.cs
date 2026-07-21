using MyDICollection.Resources;
using System.Globalization;

namespace MyDICollection.Services
{
    public class LocalizationService : ILocalizationService
    {
        public void SetCulture(string languageCode)
        {
            var culture = languageCode switch
            {
                "es" => new CultureInfo("es-MX"),
                "en" => new CultureInfo("en-US"),
                _ => Thread.CurrentThread.CurrentCulture
            };

            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            AppResource.Culture = culture;
        }

        public string Translate(string key) =>
            AppResource.ResourceManager.GetString(key, AppResource.Culture) ?? $"[{key}]";

    }
}