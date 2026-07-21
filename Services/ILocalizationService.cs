namespace MyDICollection.Services
{
    public interface ILocalizationService
    {
        void SetCulture(string languageCode);
        string Translate(string key);
    }
}