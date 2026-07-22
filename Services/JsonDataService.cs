using System.Text.Json;

namespace MyDICollection.Services
{
    public class JsonDataService : IJsonDataService
    {
        // Catálogo: SIEMPRE se lee fresco del paquete, nunca se copia a AppData.
        // Así cualquier actualización de la app trae el catálogo actualizado automáticamente.
        public async Task<T> ReadJsonFileAsync<T>(string fileName)
        {
            try
            {
                using Stream stream = await FileSystem.OpenAppPackageFileAsync(fileName);
                using StreamReader reader = new StreamReader(stream);
                string jsonContents = await reader.ReadToEndAsync();
                return JsonSerializer.Deserialize<T>(jsonContents) ?? default;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error leyendo catálogo: {ex.Message}");
                return default;
            }
        }

        // Datos del usuario: vive en AppDataDirectory, sobrevive actualizaciones de la app
        public async Task<T> ReadUserDataAsync<T>(string fileName) where T : new()
        {
            try
            {
                string localPath = Path.Combine(FileSystem.AppDataDirectory, fileName);

                if (!File.Exists(localPath))
                    return new T(); // primera vez → diccionario/objeto vacío

                string jsonContents = await File.ReadAllTextAsync(localPath);
                return JsonSerializer.Deserialize<T>(jsonContents) ?? new T();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error leyendo userdata: {ex.Message}");
                return new T();
            }
        }

        public async Task WriteUserDataAsync<T>(string fileName, T data)
        {
            string localPath = Path.Combine(FileSystem.AppDataDirectory, fileName);
            using var stream = File.Create(localPath);
            await JsonSerializer.SerializeAsync(stream, data, new JsonSerializerOptions
            {
                WriteIndented = true
            });
        }
    }
}