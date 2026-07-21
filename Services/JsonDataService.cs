using System.Text.Json;

namespace MyDICollection.Services
{
    public class JsonDataService : IJsonDataService
    {
        public async Task<T> ReadJsonFileAsync<T>(string fileName)
        {
            try
            {
                string localPath = Path.Combine(FileSystem.AppDataDirectory, fileName);
                string jsonContents;

                if (File.Exists(localPath))
                {
                    // Ya existe una copia local (con tus cambios guardados) → leemos de ahí
                    jsonContents = await File.ReadAllTextAsync(localPath);
                }
                else
                {
                    // Primera vez → leemos del paquete (Raw Assets) y creamos la copia local
                    using Stream packageStream = await FileSystem.OpenAppPackageFileAsync(fileName);
                    using StreamReader reader = new StreamReader(packageStream);
                    jsonContents = await reader.ReadToEndAsync();

                    // Guardamos la copia inicial en AppDataDirectory para futuras escrituras
                    await File.WriteAllTextAsync(localPath, jsonContents);
                }

                return JsonSerializer.Deserialize<T>(jsonContents) ?? default;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error leyendo JSON: {ex.Message}");
                return default;
            }
        }

        public async Task WriteJsonFileAsync<T>(string fileName, T data)
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
