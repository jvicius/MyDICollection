using MyDICollection.Models;

namespace MyDICollection.Services
{
    public interface ILogrosService
    {
        // Trae el catálogo estático (las reglas)
        Task<List<LogroDefinicion>> ObtenerCatalogoLogrosAsync();

        // Trae lo que el usuario ya ganó
        Task<List<LogroUsuario>> ObtenerLogrosDesbloqueadosAsync();

        // ¡El Motor! Recibe el inventario actual y revisa si ganamos algo
        Task<List<LogroDefinicion>> EvaluarLogrosAsync(List<FiguraModel> inventarioUsuario);
    }
}