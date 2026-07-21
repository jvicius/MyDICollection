using MyDICollection.Models;
using MyDICollection.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MyDICollection.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private const string FileName = "dbmyinfinity.json";
        private const string TodosLabel = "Todos";

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private readonly IJsonDataService _jsonDataService;

        // Lista maestra (sin filtrar) — nunca se muestra directo en la UI
        private List<FiguraModel> _allFigures = new();

        public ICommand IncrementarCommand { get; }
        public ICommand DecrementarCommand { get; }
        public ICommand AbrirWikiCommand { get; }

        public MainPageViewModel(IJsonDataService jsonDataService)
        {
            _jsonDataService = jsonDataService;

            IncrementarCommand = new Command<FiguraModel>(async (figura) => await CambiarCantidadAsync(figura, 1));
            DecrementarCommand = new Command<FiguraModel>(async (figura) => await CambiarCantidadAsync(figura, -1));
            AbrirWikiCommand = new Command<FiguraModel>(async (figura) => await AbrirWikiAsync(figura));

            LoadDataAsync();
        }

        // ---------- Colección mostrada en el CollectionView (ya filtrada) ----------
        private ObservableCollection<FiguraModel> _figures = new();
        public ObservableCollection<FiguraModel> Figures
        {
            get => _figures;
            set { if (_figures != value) { _figures = value; OnPropertyChanged(); } }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set { if (_isBusy != value) { _isBusy = value; OnPropertyChanged(); } }
        }

        // ---------- Opciones disponibles para cada Picker ----------
        public ObservableCollection<string> OpcionesObtenido { get; } = new() { TodosLabel, "Obtenido", "No obtenido" };
        public ObservableCollection<string> OpcionesTipo { get; } = new();
        public ObservableCollection<string> OpcionesVersion { get; } = new();
        public ObservableCollection<string> OpcionesFranquicia { get; } = new();

        // ---------- Filtros seleccionados ----------
        private string _filtroObtenido = TodosLabel;
        public string FiltroObtenido
        {
            get => _filtroObtenido;
            set { if (_filtroObtenido != value) { _filtroObtenido = value; OnPropertyChanged(); AplicarFiltros(); } }
        }

        private string _filtroTipo = TodosLabel;
        public string FiltroTipo
        {
            get => _filtroTipo;
            set { if (_filtroTipo != value) { _filtroTipo = value; OnPropertyChanged(); AplicarFiltros(); } }
        }

        private string _filtroVersion = TodosLabel;
        public string FiltroVersion
        {
            get => _filtroVersion;
            set { if (_filtroVersion != value) { _filtroVersion = value; OnPropertyChanged(); AplicarFiltros(); } }
        }

        private string _filtroFranquicia = TodosLabel;
        public string FiltroFranquicia
        {
            get => _filtroFranquicia;
            set { if (_filtroFranquicia != value) { _filtroFranquicia = value; OnPropertyChanged(); AplicarFiltros(); } }
        }

        private async Task LoadDataAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                await Task.Delay(500);
                var loadedFigures = await _jsonDataService.ReadJsonFileAsync<List<FiguraModel>>(FileName);
                _allFigures = loadedFigures ?? new List<FiguraModel>();

                CargarOpcionesDeFiltro();
                AplicarFiltros();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al procesar el ViewModel: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void CargarOpcionesDeFiltro()
        {
            OpcionesTipo.Clear();
            OpcionesTipo.Add(TodosLabel);
            foreach (var tipo in _allFigures.Select(f => f.Tipo).Distinct().OrderBy(t => t))
                OpcionesTipo.Add(tipo);

            OpcionesVersion.Clear();
            OpcionesVersion.Add(TodosLabel);
            foreach (var version in _allFigures.Select(f => f.Version).Distinct().OrderBy(v => v))
                OpcionesVersion.Add(version);

            OpcionesFranquicia.Clear();
            OpcionesFranquicia.Add(TodosLabel);
            foreach (var franquicia in _allFigures.Select(f => f.Franquicia).Distinct().OrderBy(f => f))
                OpcionesFranquicia.Add(franquicia);
        }

        private void AplicarFiltros()
        {
            IEnumerable<FiguraModel> query = _allFigures;

            if (FiltroObtenido == "Obtenido")
                query = query.Where(f => f.Obtenido);
            else if (FiltroObtenido == "No obtenido")
                query = query.Where(f => !f.Obtenido);

            if (!string.IsNullOrEmpty(FiltroTipo) && FiltroTipo != TodosLabel)
                query = query.Where(f => f.Tipo == FiltroTipo);

            if (!string.IsNullOrEmpty(FiltroVersion) && FiltroVersion != TodosLabel)
                query = query.Where(f => f.Version == FiltroVersion);

            if (!string.IsNullOrEmpty(FiltroFranquicia) && FiltroFranquicia != TodosLabel)
                query = query.Where(f => f.Franquicia == FiltroFranquicia);

            Figures = new ObservableCollection<FiguraModel>(query);
        }

        private async Task CambiarCantidadAsync(FiguraModel figura, int delta)
        {
            if (figura is null) return;

            // Buscamos en la lista MAESTRA, no en la filtrada, para no perder referencia
            var figuraEnLista = _allFigures.FirstOrDefault(f => f.Id == figura.Id);
            if (figuraEnLista is null) return;

            int nuevaCantidad = figuraEnLista.Cantidad + delta;
            if (nuevaCantidad < 0) nuevaCantidad = 0;

            figuraEnLista.Cantidad = nuevaCantidad;
            figuraEnLista.Obtenido = nuevaCantidad > 0;

            try
            {
                await _jsonDataService.WriteJsonFileAsync(FileName, _allFigures);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al guardar cambios: {ex.Message}");
            }

            // Si el filtro de "Obtenido" está activo, puede que la figura ya no deba mostrarse
            AplicarFiltros();
        }

        private async Task AbrirWikiAsync(FiguraModel figura)
        {
            if (figura is null || string.IsNullOrWhiteSpace(figura.WikiUrl)) return;

            try
            {
                await Launcher.Default.OpenAsync(new Uri(figura.WikiUrl));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al abrir la wiki: {ex.Message}");
            }
        }
    }
}