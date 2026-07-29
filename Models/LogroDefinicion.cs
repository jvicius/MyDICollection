using MyDICollection.Helpers;
using System.Text.Json.Serialization;

namespace MyDICollection.Models
{
    public class LogroDefinicion
    {
        public string Id { get; set; }
        public string CategoriaItem { get; set; }
        public string TipoFiltro { get; set; }
        public string ValorFiltro { get; set; }

        // Los campos de la BD
        public string Titulo { get; set; }
        public string TituloEn { get; set; }
        public string Descripcion { get; set; }
        public string DescripcionEn { get; set; }
        public string BadgeImage { get; set; }

        // 💥 PROPIEDADES MÁGICAS PARA LA UI 💥
        // Le ponemos [JsonIgnore] para que el deserializador no intente buscarlas en el JSON

        [JsonIgnore]
        public string TituloMostrado =>
            Settings.LanguageSettings == "en" ? TituloEn : Titulo;

        [JsonIgnore]
        public string DescripcionMostrada =>
            Settings.LanguageSettings == "en" ? DescripcionEn : Descripcion;
    }
}