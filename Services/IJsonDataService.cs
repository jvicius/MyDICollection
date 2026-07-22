namespace MyDICollection.Services
{
    public interface IJsonDataService
    {
        Task<T> ReadJsonFileAsync<T>(string fileName);          // catálogo — siempre del paquete
        Task<T> ReadUserDataAsync<T>(string fileName) where T : new(); // datos usuario — de AppData
        Task WriteUserDataAsync<T>(string fileName, T data);    // guarda datos usuario en AppData
    }
}
