// Models/FiguraModel.cs
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace MyDICollection.Models
{
    // Datos del CATÁLOGO — vienen siempre del paquete, son de solo lectura para el usuario
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

        // --- Estos 3 campos son "editables" por el usuario, pero YA NO se leen/escriben ---
        // --- directo de este JSON: se hidratan desde FiguraUserData en tiempo de carga ---
        private bool _obtenido;
        [JsonIgnore] // 👈 ya no viene del catálogo
        public bool Obtenido
        {
            get => _obtenido;
            set { if (_obtenido != value) { _obtenido = value; OnPropertyChanged(); } }
        }

        private int _cantidad;
        [JsonIgnore]
        public int Cantidad
        {
            get => _cantidad;
            set { if (_cantidad != value) { _cantidad = value; OnPropertyChanged(); } }
        }

        private ObservableCollection<string> _nfcCodes = new();
        [JsonIgnore]
        public ObservableCollection<string> NfcCodes
        {
            get => _nfcCodes;
            set { if (_nfcCodes != value) { _nfcCodes = value; OnPropertyChanged(); } }
        }
    }
}