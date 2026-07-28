using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mopups.Services;
using MyDICollection.Helpers;
using MyDICollection.Helpers.Extensions;
using MyDICollection.Helpers.Fonts;
using MyDICollection.Models;
using MyDICollection.Popups;
using MyDICollection.Resources;
using MyDICollection.Services;
using System.Collections.ObjectModel;

namespace MyDICollection.ViewModels
{
    // 1. Debe ser "partial" y heredar de ObservableObject
    public partial class MainPageViewModel : ObservableObject
    {
        public Color statusBarColor = Color.FromArgb("#EB1937");
        public Color navigationBarColor = Color.FromArgb("#EB1937");

        private const string CatalogFileName = "dbmyinfinity.json";
        private const string UserDataFileName = "userdata.json";

        private readonly IJsonDataService _jsonDataService;
        private readonly IPopupPageService _popupPageService;
        protected StatusBarService StatusBarService { get; set; }

        private List<FiguraModel> _allFigures = new();
        private Dictionary<string, FiguraUserData> _userData = new();

        public ObservableCollection<string> OpcionesObtenido { get; } = new() { AppResource.All, AppResource.Owned, AppResource.Missing };
        public ObservableCollection<string> OpcionesTipo { get; } = new();
        public ObservableCollection<string> OpcionesVersion { get; } = new();
        public ObservableCollection<string> OpcionesFranquicia { get; } = new();

        // 2. MAGIA: [ObservableProperty] genera los Getters y Setters públicos por ti.
        // Y para ejecutar algo cuando cambia (tu AplicarFiltros), usamos este método partial que se auto-engancha.

        [ObservableProperty]
        private string _filtroObtenido = AppResource.All;
        partial void OnFiltroObtenidoChanged(string value) => AplicarFiltros();

        [ObservableProperty]
        private string _filtroTipo = AppResource.All;
        partial void OnFiltroTipoChanged(string value) => AplicarFiltros();

        [ObservableProperty]
        private string _filtroVersion = AppResource.All;
        partial void OnFiltroVersionChanged(string value) => AplicarFiltros();

        [ObservableProperty]
        private string _filtroFranquicia = AppResource.All;
        partial void OnFiltroFranquiciaChanged(string value) => AplicarFiltros();

        [ObservableProperty]
        private ObservableCollection<FiguraModel> _figures = new();

        public bool MostrarLista => !IsBusy && (SelectedMenuFigures || SelectedMenuDiscs);

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(MostrarLista))]
        private bool _isBusy;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(MostrarLista))]
        private bool _selectedMenuFigures;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(MostrarLista))]
        private bool _selectedMenuDiscs;

        [ObservableProperty]
        private bool _selectedMenuArchi;

        [ObservableProperty]
        private bool _selectedMenuSettings;

        [ObservableProperty]
        private bool _selectedMenuHome;

        [ObservableProperty]
        private int _totalPiezasFisicas;

        [ObservableProperty]
        private string _textoProgresoColeccion;

        [ObservableProperty]
        private double _porcentajeColeccion;
        [ObservableProperty]
        private string _iconInfo = FontAwesomeIcons.Figura2;

        [ObservableProperty]
        private ObservableCollection<MenuOpcion> _opcionesMenu;

        public MainPageViewModel(IJsonDataService jsonDataService, IPopupPageService popupPageService, StatusBarService statusBarService)
        {
            _jsonDataService = jsonDataService;
            _popupPageService = popupPageService;
            StatusBarService = statusBarService;

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                SetStatusBarColors();
                SetupMenu();
                SelectedMenuFigures = true;
                await LoadDataAsync();
            });
            _popupPageService = popupPageService;
        }

        private void SetupMenu()
        {
            // Inicializamos el menú
            OpcionesMenu = new ObservableCollection<MenuOpcion>
            {
                // Nota: Cambia los nombres de los íconos por los que tengas en tu clase FontAwesomeIcons
                new MenuOpcion { Icono = FontAwesomeIcons.InfoCircle, Texto = "Acerca del App" },
                new MenuOpcion { Icono = FontAwesomeIcons.Globe, Texto = "Cambio de Idioma" },
                new MenuOpcion { Icono = FontAwesomeIcons.Handshake, Texto = "Contribuciones" }
            };
        }
        [RelayCommand]
        private async Task OpcionSeleccionadaAsync(MenuOpcion opcion)
        {
            if (opcion == null) return;
        }

        [RelayCommand]
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

            if (value == "MyFigures")
            {
                IconInfo = FontAwesomeIcons.Figura2;
            }

            if (value == "MyDiscs")
            {
                IconInfo = FontAwesomeIcons.PowerDisc3;
            }

            if (value == "MyFigures" || value == "MyDiscs")
            {
                await LoadDataAsync();
            }
        }

        private void SetActiveMenu(string menuName)
        {
            SelectedMenuFigures = menuName == "MyFigures";
            SelectedMenuDiscs = menuName == "MyDiscs";
            SelectedMenuArchi = menuName == "Achievements";
            SelectedMenuSettings = menuName == "Settings";
            SelectedMenuHome = menuName == "Home";
        }

        [RelayCommand]
        private async Task IncrementarAsync(FiguraModel figura)
        {
            await CambiarCantidadAsync(figura, 1);
        }

        [RelayCommand]
        private async Task DecrementarAsync(FiguraModel figura)
        {
            await CambiarCantidadAsync(figura, -1);
        }

        [RelayCommand]
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

        [RelayCommand]
        private async Task AbrirDetalleAsync(FiguraModel figura)
        {
            if (figura == null) return;

            var navParams = new NavigationParameters
                {
                    { "FiguraActual", figura }
                };

            var resultado = await _popupPageService.ShowPopupAsync<FiguraInfoPopup, FiguraInfoViewModel, bool>(navParams);
        }

        private async Task LoadDataAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                await Task.Delay(500);

                // 1. Cargar catálogo base
                var catalogo = await _jsonDataService.ReadJsonFileAsync<List<FiguraModel>>(CatalogFileName);
                IEnumerable<FiguraModel> query = catalogo ?? new List<FiguraModel>();

                // 2. Filtrar por menú ACTIVO
                if (SelectedMenuFigures)
                    query = query.Where(w => w.Tipo == AppResource.Figure);
                else if (SelectedMenuDiscs)
                    query = query.Where(w => w.Tipo == AppResource.PowerDisc);

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

                // 5. Cargar progreso del usuario
                _userData = await _jsonDataService.ReadUserDataAsync<Dictionary<string, FiguraUserData>>(UserDataFileName);

                foreach (var figura in _allFigures)
                {
                    if (_userData.TryGetValue(figura.Id, out var datosUsuario))
                    {
                        figura.Obtenido = datosUsuario.Obtenido;
                        figura.Cantidad = datosUsuario.Cantidad;
                        figura.NfcCodes = new ObservableCollection<string>(datosUsuario.NfcCodes ?? new List<string>());
                    }
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
            OpcionesTipo.Clear();
            OpcionesTipo.Add(AppResource.All);
            foreach (var tipo in _allFigures.Select(f => f.Tipo).Distinct().OrderBy(t => t))
                OpcionesTipo.Add(tipo);

            OpcionesVersion.Clear();
            OpcionesVersion.Add(AppResource.All);
            foreach (var version in _allFigures.Select(f => f.Version).Distinct().OrderBy(v => v))
                OpcionesVersion.Add(version);

            OpcionesFranquicia.Clear();
            OpcionesFranquicia.Add(AppResource.All);
            foreach (var franquicia in _allFigures.Select(f => f.Franquicia).Distinct().OrderBy(f => f))
                OpcionesFranquicia.Add(franquicia);
        }

        private void AplicarFiltros()
        {
            // 1. Obtenemos la lista filtrada por Universo (Marvel, Disney, etc.)
            var queryBase = ObtenerListaBaseFiltrada();

            // 2. 💥 Actualizamos los números con esa lista base
            ActualizarEstadisticas(queryBase);

            // 3. Aplicamos el filtro visual de Obtenidas/Faltantes
            IEnumerable<FiguraModel> queryVisual = queryBase;

            if (FiltroObtenido == AppResource.Owned)
                queryVisual = queryVisual.Where(f => f.Obtenido);
            else if (FiltroObtenido == AppResource.Missing)
                queryVisual = queryVisual.Where(f => !f.Obtenido);

            // 4. Mandamos a repintar la pantalla
            Figures = new ObservableCollection<FiguraModel>(queryVisual);
        }

        private async Task CambiarCantidadAsync(FiguraModel figura, int delta)
        {
            if (figura is null) return;

            var figuraEnLista = _allFigures.FirstOrDefault(f => f.Id == figura.Id);
            if (figuraEnLista is null) return;

            int nuevaCantidad = figuraEnLista.Cantidad + delta;
            if (nuevaCantidad < 0) nuevaCantidad = 0;

            var refrescar = false;
            if (FiltroObtenido == AppResource.Owned && nuevaCantidad == 0) refrescar = true;
            if (FiltroObtenido == AppResource.Missing && nuevaCantidad > 0) refrescar = true;

            figuraEnLista.Cantidad = nuevaCantidad;
            figuraEnLista.Obtenido = nuevaCantidad > 0;

            await GuardarProgresoAsync(figuraEnLista);

            if (refrescar)
            {
                AplicarFiltros();
            }
            else
            {
                // Si no recargamos la lista visual para ahorrar memoria, de todos modos actualizamos los numeritos de arriba
                ActualizarEstadisticas(ObtenerListaBaseFiltrada());
            }
        }

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

        private IEnumerable<FiguraModel> ObtenerListaBaseFiltrada()
        {
            IEnumerable<FiguraModel> query = _allFigures;

            if (!string.IsNullOrEmpty(FiltroTipo) && FiltroTipo != AppResource.All)
                query = query.Where(f => f.Tipo == FiltroTipo);

            if (!string.IsNullOrEmpty(FiltroVersion) && FiltroVersion != AppResource.All)
                query = query.Where(f => f.Version == FiltroVersion);

            if (!string.IsNullOrEmpty(FiltroFranquicia) && FiltroFranquicia != AppResource.All)
                query = query.Where(f => f.Franquicia == FiltroFranquicia);

            return query;
        }
        private void ActualizarEstadisticas(IEnumerable<FiguraModel> listaFiltrada)
        {
            if (listaFiltrada == null || !listaFiltrada.Any())
            {
                TotalPiezasFisicas = 0;
                TextoProgresoColeccion = "0/0";
                PorcentajeColeccion = 0; // Se resetea si no hay nada
                return;
            }

            TotalPiezasFisicas = listaFiltrada.Sum(f => f.Cantidad);

            var unicas = listaFiltrada.Count(f => f.Obtenido);
            var total = listaFiltrada.Count();

            TextoProgresoColeccion = $"{unicas}/{total}";

            // 💥 Calculamos el porcentaje (de 0.0 a 1.0)
            PorcentajeColeccion = total == 0 ? 0 : (double)unicas / total;
        }

        public void SetStatusBarColors()
        {
#if ANDROID
            StatusBarService.SetSystemBars(
            lightStatusBarColor: statusBarColor,
            darkStatusBarColor: statusBarColor,
            lightNavigationBarColor: navigationBarColor,
            darkNavigationBarColor: navigationBarColor,
            animate: false
        );
#endif
        }

    }
}