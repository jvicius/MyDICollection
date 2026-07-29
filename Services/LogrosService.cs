using System.Text.Json;
using MyDICollection.Models;

namespace MyDICollection.Services
{
    public class LogrosService : ILogrosService
    {
        private readonly IJsonDataService _jsonDataService;

        // Ruta donde guardaremos el progreso del usuario en su teléfono
        private readonly string _userLogrosPath = Path.Combine(FileSystem.AppDataDirectory, "LogrosUsuario.json");

        public LogrosService(IJsonDataService jsonDataService)
        {
            _jsonDataService = jsonDataService;
        }

        public async Task<List<LogroDefinicion>> ObtenerCatalogoLogrosAsync()
        {
            // Leemos el catálogo de logros (asumiendo que se llama así tu archivo en Raw)
            var catalogo = await _jsonDataService.ReadJsonFileAsync<List<LogroDefinicion>>("dblogros.json");
            return catalogo ?? new List<LogroDefinicion>();
        }

        public async Task<List<LogroUsuario>> ObtenerLogrosDesbloqueadosAsync()
        {
            if (!File.Exists(_userLogrosPath))
                return new List<LogroUsuario>();

            try
            {
                var json = await File.ReadAllTextAsync(_userLogrosPath);
                return JsonSerializer.Deserialize<List<LogroUsuario>>(json) ?? new List<LogroUsuario>();
            }
            catch
            {
                return new List<LogroUsuario>();
            }
        }

        public async Task<List<LogroDefinicion>> EvaluarLogrosAsync(List<FiguraModel> inventarioUsuario)
        {
            var logrosRecienDesbloqueados = new List<LogroDefinicion>();

            // 1. Cargamos todo
            var catalogoLogros = await ObtenerCatalogoLogrosAsync();
            var logrosUsuario = await ObtenerLogrosDesbloqueadosAsync();

            // Necesitamos el catálogo base de figuras para saber los totales
            var catalogoFigurasBase = await _jsonDataService.ReadJsonFileAsync<List<FiguraModel>>("dbmyinfinity.json")
                                      ?? new List<FiguraModel>();

            // 2. Filtramos los que YA están desbloqueados
            var idsDesbloqueados = logrosUsuario.Select(l => l.LogroId).ToHashSet();
            var logrosPendientes = catalogoLogros.Where(l => !idsDesbloqueados.Contains(l.Id)).ToList();

            bool huboNuevos = false;

            // 3. Evaluamos solo los pendientes
            foreach (var logro in logrosPendientes)
            {
                // ¿Cuántos existen en TOTAL en el juego según esta regla?
                int metaTotal = catalogoFigurasBase.Count(f =>
                    f.Tipo == logro.CategoriaItem && CumpleFiltro(f, logro));

                // ¿Cuántos tiene el USUARIO que cumplan esta regla?
                // ¿Cuántos tiene el USUARIO que cumplan esta regla?
                int progresoUsuario = inventarioUsuario.Count(f =>
                    f.Tipo == logro.CategoriaItem &&
                    CumpleFiltro(f, logro) &&
                    f.Obtenido); 

                // 💥 VEREDICTO 💥
                if (metaTotal > 0 && progresoUsuario >= metaTotal)
                {
                    // ¡Logro desbloqueado!
                    logrosUsuario.Add(new LogroUsuario
                    {
                        LogroId = logro.Id,
                        FechaDesbloqueo = DateTime.Now
                    });

                    logrosRecienDesbloqueados.Add(logro);
                    huboNuevos = true;
                }
            }

            // 4. Si ganó algo, guardamos en el JSON del usuario
            if (huboNuevos)
            {
                var json = JsonSerializer.Serialize(logrosUsuario);
                await File.WriteAllTextAsync(_userLogrosPath, json);
            }

            // Regresamos los nuevos para que la UI pueda festejar
            return logrosRecienDesbloqueados;
        }

        // El motorcito dinámico con switch que platicamos
        private bool CumpleFiltro(FiguraModel figura, LogroDefinicion logro)
        {
            return logro.TipoFiltro switch
            {
                "Todos" => true,
                "Version" => figura.Version == logro.ValorFiltro,
                "Franquicia" => figura.Franquicia == logro.ValorFiltro,
                "Especial" => figura.EdicionEspecial == logro.ValorFiltro,
                _ => false
            };
        }
    }
}