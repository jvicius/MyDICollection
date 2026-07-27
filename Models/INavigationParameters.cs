namespace MyDICollection.Models
{
    public interface INavigationParameters : IEnumerable<KeyValuePair<string, object>>
    {
        void Add(string key, object value);
        T GetValue<T>(string key);
        bool TryGetValue<T>(string key, out T value);
        bool ContainsKey(string key);
    }
}
