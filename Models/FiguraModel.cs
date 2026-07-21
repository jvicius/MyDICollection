using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace MyDICollection.Models
{
    public class FiguraModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("nombre")]
        public string Nombre { get; set; }

        [JsonPropertyName("tipo")]
        public string Tipo { get; set; }

        [JsonPropertyName("version")]
        public string Version { get; set; }

        [JsonPropertyName("franquicia")]
        public string Franquicia { get; set; }

        [JsonPropertyName("imagen_url")]
        public string ImagenUrl { get; set; }

        [JsonPropertyName("wiki_url")]
        public string WikiUrl { get; set; }

        private bool _obtenido;
        [JsonPropertyName("obtenido")]
        public bool Obtenido
        {
            get => _obtenido;
            set { if (_obtenido != value) { _obtenido = value; OnPropertyChanged(); } }
        }

        private int _cantidad;
        [JsonPropertyName("cantidad")]
        public int Cantidad
        {
            get => _cantidad;
            set { if (_cantidad != value) { _cantidad = value; OnPropertyChanged(); } }
        }
    }
}