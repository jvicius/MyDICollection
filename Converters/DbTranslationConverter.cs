using System.Globalization;

namespace MyDICollection.Converters
{
    public class DbTranslationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string originalString || string.IsNullOrEmpty(originalString))
                return string.Empty;

            bool isEnglish = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "en";

            if (!isEnglish)
            {
                return originalString;
            }

            return originalString switch
            {
                // ---- Tipos ----
                "Figura" => "Figure",
                "Power Disc" => "Power Disc",

                // ---- Franquicias 1.0 ----
                "Piratas del Caribe" => "Pirates of the Caribbean",
                "Los Increibles" => "The Incredibles",
                "Monsters University" => "Monsters University", 
                "Cars" => "Cars", 
                "El Llanero Solitario" => "The Lone Ranger",
                "El extraño mundo de Jack" => "The Nightmare Before Christmas",
                "Toy Story" => "Toy Story", 
                "Ralph el Demoledor" => "Wreck-It Ralph",
                "Enredados" => "Tangled",
                "Frozen" => "Frozen", 
                "Phineas y Ferb" => "Phineas and Ferb",

                // ---- Franquicias 2.0 ----
                "Marvel" => "Marvel", 
                "Grandes Héroes" => "Big Hero 6",
                "Tron" => "Tron", 
                "Maléfica" => "Maleficent",
                "Lilo & Stitch" => "Lilo & Stitch", 
                "Peter Pan" => "Peter Pan", 

                // ---- Franquicias 3.0 ----
                "Star Wars" => "Star Wars", 
                "Intensa Mente" => "Inside Out",

                // ---- Franquicias Generales/Clásicos ----
                "Disney Clasicos" => "Disney Classics",

                // ---- Casos Especiales/Nuevos ----
                "Por Asignar" => "Unassigned",

                "Un Gran Dinosaurio" => "The Good Dinosaur",
                "Buscando a Nemo" => "Finding Nemo",
                "El Libro de la Selva" => "The Jungle Book",
                "Alicia en el Pais de las Maravillas" => "Alice in Wonderland",

                "Todos" => "All",

                // Fallback:
                _ => originalString
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
