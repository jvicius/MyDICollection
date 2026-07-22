using System.Globalization;

namespace MyDICollection.Converters
{
    public class DbTranslationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string originalString || string.IsNullOrEmpty(originalString))
                return string.Empty;

            // Detectamos si el celular está en inglés
            bool isEnglish = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "en";

            if (!isEnglish)
            {
                // Si está en español (o cualquier otro idioma), pasamos el texto original
                return originalString;
            }

            // Mapeo exhaustivo de traducciones al inglés basado en tu JSON
            return originalString switch
            {
                // ---- Tipos ----
                "Figura" => "Figure",
                "Power Disc" => "Power Disc",

                // ---- Franquicias 1.0 ----
                "Piratas del Caribe" => "Pirates of the Caribbean",
                "Los Increibles" => "The Incredibles",
                "Monsters University" => "Monsters University", // Se queda igual
                "Cars" => "Cars", // Se queda igual
                "El Llanero Solitario" => "The Lone Ranger",
                "El extraño mundo de Jack" => "The Nightmare Before Christmas",
                "Toy Story" => "Toy Story", // Se queda igual
                "Ralph el Demoledor" => "Wreck-It Ralph",
                "Enredados" => "Tangled",
                "Frozen" => "Frozen", // Se queda igual
                "Phineas y Ferb" => "Phineas and Ferb",

                // ---- Franquicias 2.0 ----
                "Marvel" => "Marvel", // Se queda igual
                "Grandes Héroes" => "Big Hero 6",
                "Tron" => "Tron", // Se queda igual
                "Maléfica" => "Maleficent",
                "Lilo & Stitch" => "Lilo & Stitch", // Se queda igual
                "Peter Pan" => "Peter Pan", // Se queda igual

                // ---- Franquicias 3.0 ----
                "Star Wars" => "Star Wars", // Se queda igual
                "Intensa Mente" => "Inside Out",

                // ---- Franquicias Generales/Clásicos ----
                "Disney Clasicos" => "Disney Classics",

                // ---- Casos Especiales/Nuevos ----
                "Por Asignar" => "Unassigned",

                "Todos" => "All",

                // Fallback: si no está en el diccionario, lo pasamos tal cual
                _ => originalString
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
