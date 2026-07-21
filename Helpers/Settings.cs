namespace MyDICollection.Helpers
{
    public static class Settings
    {
        private static readonly string StringDefault = string.Empty;

        private const string LanguageKey = "language_key";
        public static string LanguageSettings
        {
            get => Preferences.Get(LanguageKey, StringDefault);
            set => Preferences.Set(LanguageKey, value);
        }
    }
}
