using MyDICollection.Converters;

namespace MyDICollection.Helpers.Extensions
{
    public static class StringExtensions
    {
        public static string ToCurrentLanguageTraslate(this string texto)
        {
            if (string.IsNullOrEmpty(texto))
                return string.Empty;

            var traductor = new DbTranslationConverter();

            return traductor.Convert(
                    value: texto,
                    targetType: typeof(string),
                    parameter: null,
                    culture: System.Globalization.CultureInfo.CurrentUICulture).ToString();
        }
    }
}
