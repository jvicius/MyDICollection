using System.Collections;

namespace MyDICollection.Models
{
    public class NavigationParameters : INavigationParameters
    {
        private readonly Dictionary<string, object> _parameters = new();

        public void Add(string key, object value)
        {
            _parameters[key] = value;
        }

        public T GetValue<T>(string key)
        {
            if (_parameters.TryGetValue(key, out var value))
            {
                return (T)value;
            }
            return default;
        }

        public bool TryGetValue<T>(string key, out T value)
        {
            if (_parameters.TryGetValue(key, out var objValue) && objValue is T typedValue)
            {
                value = typedValue;
                return true;
            }

            value = default;
            return false;
        }

        public bool ContainsKey(string key) => _parameters.ContainsKey(key);

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => _parameters.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _parameters.GetEnumerator();
    }
}
