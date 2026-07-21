using MyDICollection.Resources;
using System.ComponentModel;

namespace MyDICollection.Models
{
    public class Localization : INotifyPropertyChanged
    {
        public static Localization Instance { get; } = new Localization();

        public string this[string key] =>
            AppResource.ResourceManager.GetString(key, AppResource.Culture) ?? $"[{key}]";

        public event PropertyChangedEventHandler? PropertyChanged;

        public void Reload()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }
    }
}