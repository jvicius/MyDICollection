namespace MyDICollection.Services
{
    public interface IJsonDataService
    {
        Task<T> ReadJsonFileAsync<T>(string fileName);
        Task WriteJsonFileAsync<T>(string fileName, T data);
    }
}
