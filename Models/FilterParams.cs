namespace MyDICollection.Models
{
    public class FilterParams
    {
        // Las listas de opciones (para llenar los Pickers)
        public List<string> OpcionesObtenido { get; set; } = new();
        public List<string> OpcionesVersion { get; set; } = new();
        public List<string> OpcionesFranquicia { get; set; } = new();

        // Los valores seleccionados actualmente
        public string FiltroObtenido { get; set; }
        public string FiltroVersion { get; set; }
        public string FiltroFranquicia { get; set; }
    }
}