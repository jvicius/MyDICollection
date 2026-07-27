using MyDICollection.Helpers;
using MyDICollection.Helpers.Extensions;
using MyDICollection.Models;
using MyDICollection.Resources;
using MyDICollection.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MyDICollection.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        #region consts

        private const string CatalogFileName = "dbmyinfinity.json";
        private const string UserDataFileName = "userdata.json";

        #endregion

        #region properties and variables

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private readonly IJsonDataService _jsonDataService;

        private List<FiguraModel> _allFigures = new();
        // Diccionario Id -> datos del usuario, se guarda tal cual en userdata.json
        private Dictionary<string, FiguraUserData> _userData = new();
        public ObservableCollection<string> OpcionesObtenido { get; } = new() { AppResource.All, AppResource.Owned, AppResource.Missing };
        public ObservableCollection<string> OpcionesTipo { get; } = new();
        public ObservableCollection<string> OpcionesVersion { get; } = new();
        public ObservableCollection<string> OpcionesFranquicia { get; } = new();

        private string _filtroObtenido = AppResource.All;
        public string FiltroObtenido
        {
            get => _filtroObtenido;
            set { if (_filtroObtenido != value) { _filtroObtenido = value; OnPropertyChanged(); AplicarFiltros(); } }
        }

        private string _filtroTipo = AppResource.All;
        public string FiltroTipo
        {
            get => _filtroTipo;
            set { if (_filtroTipo != value) { _filtroTipo = value; OnPropertyChanged(); AplicarFiltros(); } }
        }

        private string _filtroVersion = AppResource.All;
        public string FiltroVersion
        {
            get => _filtroVersion;
            set { if (_filtroVersion != value) { _filtroVersion = value; OnPropertyChanged(); AplicarFiltros(); } }
        }

        private string _filtroFranquicia = AppResource.All;
        public string FiltroFranquicia
        {
            get => _filtroFranquicia;
            set { if (_filtroFranquicia != value) { _filtroFranquicia = value; OnPropertyChanged(); AplicarFiltros(); } }
        }
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

        private bool _selectedMenuFigures;
        public bool SelectedMenuFigures
        {
            get => _selectedMenuFigures;
            set
            {
                if (_selectedMenuFigures != value)
                {
                    _selectedMenuFigures = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _selectedMenuDiscs;
        public bool SelectedMenuDiscs
        {
            get => _selectedMenuDiscs;
            set
            {
                if (_selectedMenuDiscs != value)
                {
                    _selectedMenuDiscs = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _selectedMenuArchi;
        public bool SelectedMenuArchi
        {
            get => _selectedMenuArchi;
            set
            {
                if (_selectedMenuArchi != value)
                {
                    _selectedMenuArchi = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _selectedMenuSettings;
        public bool SelectedMenuSettings
        {
            get => _selectedMenuSettings;
            set
            {
                if (_selectedMenuSettings != value)
                {
                    _selectedMenuSettings = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _selectedMenuHome;
        public bool SelectedMenuHome
        {
            get => _selectedMenuHome;
            set
            {
                if (_selectedMenuHome != value)
                {
                    _selectedMenuHome = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region commands
        public ICommand IncrementarCommand { get; }
        public ICommand DecrementarCommand { get; }
        public ICommand AbrirWikiCommand { get; }
        public ICommand MenuCommand { get; }

        #endregion

        #region constructor
        public MainPageViewModel(IJsonDataService jsonDataService)
        {
            _jsonDataService = jsonDataService;

            IncrementarCommand = new Command<FiguraModel>(async (figura) => await CambiarCantidadAsync(figura, 1));
            DecrementarCommand = new Command<FiguraModel>(async (figura) => await CambiarCantidadAsync(figura, -1));
            AbrirWikiCommand = new Command<FiguraModel>(async (figura) => await AbrirWikiAsync(figura));
            MenuCommand = new Command<string>(async (value) => await AbrirMenuAsync(value));


            MainThread.BeginInvokeOnMainThread(async () =>
            {
                SelectedMenuFigures = true;
                await LoadDataAsync();
            });
        }
        private void SetActiveMenu(string menuName)
        {
            SelectedMenuFigures = menuName == "MyFigures";
            SelectedMenuDiscs = menuName == "MyDiscs";
            SelectedMenuArchi = menuName == "Achievements";
            SelectedMenuSettings = menuName == "Settings";
            SelectedMenuHome = menuName == "Home";
        }
        private async Task AbrirMenuAsync(string value)
        {
            if ((value == "MyFigures" && SelectedMenuFigures) ||
                (value == "MyDiscs" && SelectedMenuDiscs) ||
                (value == "Achievements" && SelectedMenuArchi) ||
                (value == "Settings" && SelectedMenuSettings))
                return;

            SetActiveMenu(value);

            if (value == "Home")
            {
                await Task.Delay(500);
                SelectedMenuHome = false;
                return;
            }

            if (value == "MyFigures" || value == "MyDiscs")
            {
                await LoadDataAsync();
            }
        }

        #endregion
        #region methods
        private async Task LoadDataAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                await Task.Delay(500);

                // 1. Cargar datos
                var catalogo = await _jsonDataService.ReadJsonFileAsync<List<FiguraModel>>(CatalogFileName);
                IEnumerable<FiguraModel> query = catalogo ?? new List<FiguraModel>();

                // 2. Filtrar por menú ACTIVO
                if (SelectedMenuFigures)
                    query = query.Where(w => w.Tipo == "Figura");
                else if (SelectedMenuDiscs)
                    query = query.Where(w => w.Tipo == "Disco de Poder");

                // 3. Traducir SOLO los que quedaron en el filtro
                foreach (var figura in query)
                {
                    figura.Tipo = figura.Tipo.ToCurrentLanguageTraslate();
                    figura.Franquicia = figura.Franquicia.ToCurrentLanguageTraslate();
                }

                // 4. Ordenar al final
                _allFigures = (Settings.LanguageSettings == "es")
                    ? query.OrderByDescending(x => x.Tipo).ThenBy(x => x.Version).ThenBy(x => x.Franquicia).ThenBy(x => x.Nombre).ToList()
                    : query.OrderBy(x => x.Tipo).ThenBy(x => x.Version).ThenBy(x => x.Franquicia).ThenBy(x => x.Nombre).ToList();

                // 2) Datos del usuario desde AppData
                _userData = await _jsonDataService.ReadUserDataAsync<Dictionary<string, FiguraUserData>>(UserDataFileName);

                // 3) Merge: hidratamos cada figura del catálogo con su progreso guardado
                foreach (var figura in _allFigures)
                {
                    if (_userData.TryGetValue(figura.Id, out var datosUsuario))
                    {
                        figura.Obtenido = datosUsuario.Obtenido;
                        figura.Cantidad = datosUsuario.Cantidad;
                        figura.NfcCodes = new ObservableCollection<string>(datosUsuario.NfcCodes ?? new List<string>());
                    }
                    // si no existe entrada -> se queda en default (false, 0, lista vacía)
                }

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
            // Opciones de Tipo
            OpcionesTipo.Clear();
            OpcionesTipo.Add(AppResource.All);
            var tiposUnicos = _allFigures.Select(f => f.Tipo).Distinct().OrderBy(t => t);
            foreach (var tipo in tiposUnicos)
                OpcionesTipo.Add(tipo);

            // Opciones de Versión
            OpcionesVersion.Clear();
            OpcionesVersion.Add(AppResource.All);
            var versionesUnicas = _allFigures.Select(f => f.Version).Distinct().OrderBy(v => v);
            foreach (var version in versionesUnicas)
                OpcionesVersion.Add(version);

            // Opciones de Franquicia
            OpcionesFranquicia.Clear();
            OpcionesFranquicia.Add(AppResource.All);
            var franquiciasUnicas = _allFigures.Select(f => f.Franquicia).Distinct().OrderBy(f => f);
            foreach (var franquicia in franquiciasUnicas)
                OpcionesFranquicia.Add(franquicia);
        }

        private void AplicarFiltros()
        {
            IEnumerable<FiguraModel> query = _allFigures;

            if (FiltroObtenido == AppResource.Owned)
                query = query.Where(f => f.Obtenido);
            else if (FiltroObtenido == AppResource.Missing)
                query = query.Where(f => !f.Obtenido);

            if (!string.IsNullOrEmpty(FiltroTipo) && FiltroTipo != AppResource.All)
                query = query.Where(f => f.Tipo == FiltroTipo);

            if (!string.IsNullOrEmpty(FiltroVersion) && FiltroVersion != AppResource.All)
                query = query.Where(f => f.Version == FiltroVersion);

            if (!string.IsNullOrEmpty(FiltroFranquicia) && FiltroFranquicia != AppResource.All)
                query = query.Where(f => f.Franquicia == FiltroFranquicia);

            Figures = new ObservableCollection<FiguraModel>(query);
        }

        private async Task CambiarCantidadAsync(FiguraModel figura, int delta)
        {
            if (figura is null) return;

            var refrescar = false;

            var figuraEnLista = _allFigures.FirstOrDefault(f => f.Id == figura.Id);
            if (figuraEnLista is null) return;

            int nuevaCantidad = figuraEnLista.Cantidad + delta;
            if (nuevaCantidad < 0) nuevaCantidad = 0;

            if (FiltroObtenido == AppResource.Owned && nuevaCantidad == 0)
                refrescar = true;

            if (FiltroObtenido == AppResource.Missing && nuevaCantidad > 0)
                refrescar = true;

            figuraEnLista.Cantidad = nuevaCantidad;
            figuraEnLista.Obtenido = nuevaCantidad > 0;

            await GuardarProgresoAsync(figuraEnLista);

            if(refrescar)
                AplicarFiltros();
        }

        // Guarda SOLO el diccionario de progreso (userdata.json), nunca el catálogo
        private async Task GuardarProgresoAsync(FiguraModel figura)
        {
            _userData[figura.Id] = new FiguraUserData
            {
                Obtenido = figura.Obtenido,
                Cantidad = figura.Cantidad,
                NfcCodes = figura.NfcCodes.ToList()
            };

            try
            {
                await _jsonDataService.WriteUserDataAsync(UserDataFileName, _userData);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al guardar progreso: {ex.Message}");
            }
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
    #endregion
}