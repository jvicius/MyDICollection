using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyDICollection.Helpers;
using MyDICollection.Helpers.Crypto;
using MyDICollection.Helpers.Extensions;
using MyDICollection.Helpers.Fonts;
using MyDICollection.Models;
using MyDICollection.Popups;
using MyDICollection.Resources;
using MyDICollection.Services;
using MyDICollection.Services.Nfc;
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
        private readonly ILocalizationService _localizationService;
        private readonly ILogrosService _logrosService;
        private readonly IDisneyNfcService _disneyNfcService;
        protected StatusBarService StatusBarService { get; set; }

        private List<FiguraModel> _fullListFigures = new();
        private List<FiguraModel> _allFigures = new();
        private Dictionary<string, FiguraUserData> _userData = new();

        public ObservableCollection<string> OpcionesObtenido { get; } = new() { AppResource.All, AppResource.Owned, AppResource.Missing };
        public ObservableCollection<string> OpcionesTipo { get; } = new();
        public ObservableCollection<string> OpcionesVersion { get; } = new();
        public ObservableCollection<string> OpcionesFranquicia { get; } = new();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(TieneLogros))]
        private ObservableCollection<LogroDefinicion> _logrosObtenidos = new();
        public bool TieneLogros => LogrosObtenidos != null && LogrosObtenidos.Count > 0;

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

        public bool MostrarLista => (SelectedMenuFigures || SelectedMenuDiscs);

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

        [ObservableProperty]
        private string _textoLogrosFiguras = "0/0";

        [ObservableProperty]
        private double _progresoLogrosFiguras = 0;

        [ObservableProperty]
        private string _textoLogrosDiscos = "0/0";

        [ObservableProperty]
        private double _progresoLogrosDiscos = 0;

        private bool _isMenuExecuting = false;
        private bool _IsScanFigure = false;

        public MainPageViewModel(IJsonDataService jsonDataService, IPopupPageService popupPageService, StatusBarService statusBarService, ILocalizationService localizationService, ILogrosService logrosService, IDisneyNfcService disneyNfcService)
        {
            _jsonDataService = jsonDataService;
            _popupPageService = popupPageService;
            StatusBarService = statusBarService;
            _localizationService = localizationService;
            _logrosService = logrosService;
            _disneyNfcService  = disneyNfcService;


            MainThread.BeginInvokeOnMainThread(async () =>
            {
                //borrar logros test
                //var rutaLogros = Path.Combine(FileSystem.AppDataDirectory, "LogrosUsuario.json");
                //if (File.Exists(rutaLogros))
                //{
                //    File.Delete(rutaLogros);
                //}

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
                new MenuOpcion { Icono = FontAwesomeIcons.InfoCircle, Texto = AppResource.MenuAboutApp },
                new MenuOpcion { Icono = FontAwesomeIcons.Language, Texto = AppResource.MenuChangeLanguage },
                new MenuOpcion { Icono = FontAwesomeIcons.CloudUpload, Texto = AppResource.MenuBackup },
                new MenuOpcion { Icono = FontAwesomeIcons.CloudDownload, Texto = AppResource.MenuRestore },
                new MenuOpcion { Icono = FontAwesomeIcons.Handshake, Texto = AppResource.MenuContributions },
                new MenuOpcion { Icono = FontAwesomeIcons.Bug, Texto = AppResource.MenuReportIssue } ,
            };
        }

        [RelayCommand]
        private async Task AbrirFiltrosAsync()
        {
            // Empacamos el estado actual de los filtros
            var parametrosActuales = new FilterParams
            {
                OpcionesObtenido = OpcionesObtenido.ToList(),
                OpcionesVersion = OpcionesVersion.ToList(),
                OpcionesFranquicia = OpcionesFranquicia.ToList(),
                FiltroObtenido = this.FiltroObtenido,
                FiltroVersion = this.FiltroVersion,
                FiltroFranquicia = this.FiltroFranquicia
            };

            // Usas tu servicio de inyección de dependencias para crear y pasar el modelo si es necesario,
            // o configuras el ViewModel antes de mostrarlo.
            var navParams = new NavigationParameters { { "Filtros", parametrosActuales } };
            var nuevosFiltros = await _popupPageService.ShowPopupAsync<FilterPopup, FilterViewModel, FilterParams>(navParams);

            // Si no regresó null, significa que le dio a "Aplicar"
            if (nuevosFiltros != null)
            {
                FiltroObtenido = nuevosFiltros.FiltroObtenido;
                FiltroVersion = nuevosFiltros.FiltroVersion;
                FiltroFranquicia = nuevosFiltros.FiltroFranquicia;

                AplicarFiltros();
            }
        }

        [RelayCommand]
        private async Task OpcionSeleccionadaAsync(MenuOpcion opcion)
        {
            if (opcion == null) return;

            if (opcion.Texto == AppResource.MenuReportIssue) 
            {
                await ReportarIssueAsync();
            }

            if (opcion.Texto == AppResource.MenuBackup)
            {
                await RespaldarColeccionAsync();
            }
            else if (opcion.Texto == AppResource.MenuRestore)
            {
                await RestaurarColeccionAsync();

                // 💥 AQUI METEMOS EL MOTOR DE LOGROS 💥
                await EvaluarLogros();
                // 💥 FIN DEL MOTOR DE LOGROS 💥
            }

            if (opcion.Texto == AppResource.MenuAboutApp)
            {
                var resultado = await _popupPageService.ShowPopupAsync<AboutPopup, AboutViewModel, bool>();
            }

            if (opcion.Texto == AppResource.MenuChangeLanguage)
            {
                var resultado = await _popupPageService.ShowPopupAsync<LanguagePopup, LanguageViewModel, string>();

                if (!string.IsNullOrEmpty(resultado))
                {
                    if (resultado != Settings.LanguageSettings)
                    {
                        Settings.LanguageSettings = resultado;

                        _localizationService.SetCulture(Settings.LanguageSettings);

                        await Task.Delay(500);

                        Application.Current.MainPage = new AppShell();
                    }
                }
            }

            if (opcion.Texto == AppResource.MenuContributions) // O el nombre que le hayas dado
            {
                var resultado = await _popupPageService.ShowPopupAsync<ContributionsPopup, ContributionsViewModel, bool>();
            }
        }

        private async Task ReportarIssueAsync()
        {
            try
            {
                // 1. Sacamos la info del dispositivo para que te llegue el chisme completo
                string versionApp = AppInfo.Current.VersionString;
                string plataforma = DeviceInfo.Current.Platform.ToString();

                // 2. Armamos la plantilla con Markdown
                string bodyCuerpo = $"**{AppResource.DescribeIssue}**\n\n\n" +
                    $"---\n" +
                    $"*📱 App Version: {versionApp}*\n" +
                    $"*⚙️ OS: {plataforma}*";

                // 3. Codificamos el texto para que la URL sea válida
                string encodedBody = Uri.EscapeDataString(bodyCuerpo);
                string encodedTitle = Uri.EscapeDataString("[Bug] ");

                // 4. Tu URL directa con los parámetros inyectados
                string url = $"https://github.com/jvicius/MyDICollection/issues/new?title={encodedTitle}&body={encodedBody}";

                // 5. ¡Pum! Abrimos el navegador
                await Launcher.Default.OpenAsync(new Uri(url));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al abrir GitHub: {ex.Message}");
                // Opcional: Mostrar un MostrarAlertaAsync diciendo que no se pudo abrir el navegador
            }
        }

        private async Task RespaldarColeccionAsync()
        {
            try
            {
                // 1. Buscamos dónde vive tu userdata.json actual
                string rutaArchivo = Path.Combine(FileSystem.AppDataDirectory, "userdata.json");

                if (!File.Exists(rutaArchivo))
                {
                    // Si por alguna razón no existe, no hay nada que respaldar
                    return;
                }

                // 2. Usamos el Share nativo del celular
                await Share.Default.RequestAsync(new ShareFileRequest
                {
                    Title = AppResource.BackupShareTitle, // Título del menú nativo
                    File = new ShareFile(rutaArchivo)     // Le pasamos tu JSON
                });

                await Task.Delay(500);

                await MostrarAlertaAsync(FontAwesomeIcons.CloudUpload, AppResource.BackupSuccess, Colors.Green);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al respaldar: {ex.Message}");
            }
        }

        private async Task RestaurarColeccionAsync()
        {
            try
            {
                // 1. Definimos que solo queremos que el usuario pueda elegir archivos .json
                var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.iOS, new[] { "public.json" } }, // iOS usa UTTypes
                    { DevicePlatform.Android, new[] { "application/json" } }, // Android usa MIME types
                    { DevicePlatform.WinUI, new[] { ".json" } }, // Windows usa extensiones
                    { DevicePlatform.macOS, new[] { "json" } }
                });

                // 2. Abrimos el explorador de archivos nativo
                var resultado = await FilePicker.Default.PickAsync(new PickOptions
                {
                    PickerTitle = "Selecciona tu userdata.json",
                    FileTypes = customFileType
                });

                if (resultado != null)
                {
                    await Task.Delay(500);
                    // 3. Validamos que en efecto sea un JSON (por si las moscas)
                    if (resultado.FileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                    {
                        // 4. Ubicamos dónde debe ir el archivo en la app
                        string rutaDestino = Path.Combine(FileSystem.AppDataDirectory, "userdata.json");

                        // 5. Copiamos el archivo que eligió el usuario y sobreescribimos el actual
                        using var streamOrigen = await resultado.OpenReadAsync();
                        using var streamDestino = File.Create(rutaDestino);

                        await streamOrigen.CopyToAsync(streamDestino);

                        // 6. ¡Éxito! Le avisamos al usuario. 
                        // NOTA: Como la base de datos cambió por debajo del agua, 
                        // lo más sano es pedirle que reinicie la app para que tus listas se vuelvan a cargar limpias.

                        await MostrarAlertaAsync(FontAwesomeIcons.CloudDownload, AppResource.RestoreSuccess, Colors.Green);
                    }
                    else
                    {
                        await MostrarAlertaAsync(FontAwesomeIcons.ExclamationTriangle, AppResource.RestoreError, Colors.Red);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al restaurar: {ex.Message}");
            }
        }

        protected async Task<bool> MostrarAlertaAsync(string icono, string mensaje, Color fontColor = null)
        {
            // Si no mandas color, le ponemos un rojo de advertencia por default
            fontColor ??= Colors.Red;

            var parameters = new NavigationParameters
            {
                { "Icono", icono },
                { "Mensaje", mensaje },
                { "FontColor", fontColor }
            };

            return await _popupPageService.ShowPopupAsync<AlertMessagePopup, AlertMessagePopupViewModel, bool>(parameters);
        }

        [RelayCommand]
        private async Task AbrirMenuAsync(string value)
        {
            // 2. Si ya se está ejecutando el comando, ignoramos el toque extra y salimos
            if (_isMenuExecuting)
                return;

            try
            {
                // 3. Cerramos el candado
                _isMenuExecuting = true;

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
                    _IsScanFigure = false;

                    if (!_disneyNfcService.IsSupported || !_disneyNfcService.IsAvailable)
                    {
                        await MostrarAlertaAsync(FontAwesomeIcons.ErrorTimes, AppResource.NFCNotSupported, Colors.Red);
                        return;
                    }

                    if (!_disneyNfcService.IsEnabled)
                    {
                        await MostrarAlertaAsync(FontAwesomeIcons.ExclamationTriangle, AppResource.NFCDisabled, Colors.Yellow);
                        return;
                    }

                    //// Verificamos si el cel tiene NFC y si está prendido
                    if (_disneyNfcService.IsEnabled && _disneyNfcService.IsAvailable && _disneyNfcService.IsSupported)
                    {
                        var resultado = await _popupPageService.ShowPopupAsync<NfcScannerPopup, NfcScannerViewModel, DisneyNfcUtils.DisneyFigureInfo>();

                        await Task.Delay(500);

                        if (resultado != null)
                        {
                            var item = _fullListFigures.FirstOrDefault(f => f.Modelo == resultado.InfCode);
                            if (item != null)
                            {
                                // 1. Prendemos la bandera para mostrar tu loader en pantalla
                                //IsBusy = true;

                                try
                                {
                                    // 💡 Ya no usamos MainThread.BeginInvokeOnMainThread aquí
                                    // para no ahogar la interfaz mientras guarda y calcula logros.
                                    _IsScanFigure = true;

                                    item.NfcCodes ??= new ObservableCollection<string>();

                                    bool chipYaRegistrado = _userData.ContainsKey(item.Id) && _userData[item.Id].NfcCodes.Contains(resultado.UidHex);

                                    if (!chipYaRegistrado)
                                    {
                                        // Es un chip nuevo. Lo agregamos a la lista.
                                        item.NfcCodes.Add(resultado.UidHex);

                                        // Incrementamos la cantidad y calculamos logros en segundo plano
                                        await CambiarCantidadAsync(item,1);

                                        await MostrarAlertaAsync(FontAwesomeIcons.OkCheckCircle, AppResource.AddFigure, Colors.Green);

                                        await Task.Delay(500);
                                    }
                                    
                                    item.CurrentUidHex = resultado.UidHex;
                                    await AbrirDetalleAsync(item);
                                    item.CurrentUidHex = string.Empty;
                                    _IsScanFigure = false;
                                }
                                finally
                                {
                                    // 2. Pase lo que pase, apagamos el loader al terminar
                                    //IsBusy = false;
                                }
                            }
                            else
                            {
                                // Si la figura no está en tu catálogo base (_fullListFigures)
                                await MostrarAlertaAsync(FontAwesomeIcons.NotFoundQuestion, AppResource.Figurenotfound, Colors.Orange);
                            }
                        }
                        else
                        {
                            //await MostrarAlertaAsync(FontAwesomeIcons.ExclamationTriangle, AppResource.Figurenotfound, Colors.Yellow);
                        }
                    }
                    else
                    {
                        await MostrarAlertaAsync(FontAwesomeIcons.ErrorTimes, AppResource.NFCNotSupported, Colors.Red);
                    }
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

                if (value == "Achievements")
                {
                    await CargarLogrosFamaAsync();
                }

                if (value == "MyFigures" || value == "MyDiscs")
                {
                    await LoadDataAsync();
                }
            }
            finally
            {
                // 4. Se ejecuta SIEMPRE al terminar, liberando el botón para el siguiente uso
                _isMenuExecuting = false;
            }
        }
        private async Task CargarLogrosFamaAsync()
        {
            var historialUsuario = await _logrosService.ObtenerLogrosDesbloqueadosAsync();
            var catalogoCompleto = await _logrosService.ObtenerCatalogoLogrosAsync();

            if (catalogoCompleto != null)
            {
                // 1. Calculamos los totales que existen en el juego
                int totalLogrosFiguras = catalogoCompleto.Count(l => l.CategoriaItem == "Figura");
                // Nota: Asegúrate de que "Disco de Poder" sea el texto exacto que usas en tu JSON de logros
                int totalLogrosDiscos = catalogoCompleto.Count(l => l.CategoriaItem == "Disco de Poder");

                int obtenidosFiguras = 0;
                int obtenidosDiscos = 0;
                var listaVisual = new List<LogroDefinicion>();

                if (historialUsuario != null && historialUsuario.Any())
                {
                    foreach (var progreso in historialUsuario)
                    {
                        var definicion = catalogoCompleto.FirstOrDefault(c => c.Id == progreso.LogroId);
                        if (definicion != null)
                        {
                            definicion.FechaObtenido = progreso.FechaDesbloqueo;
                            listaVisual.Add(definicion);

                            // Vamos sumando a los contadores de lo que ya obtuvo el usuario
                            if (definicion.CategoriaItem == "Figura") obtenidosFiguras++;
                            else if (definicion.CategoriaItem == "Disco de Poder") obtenidosDiscos++;
                        }
                    }

                    var logrosOrdenados = listaVisual.OrderByDescending(l => l.FechaObtenido).ToList();
                    LogrosObtenidos = new ObservableCollection<LogroDefinicion>(logrosOrdenados);
                }
                else
                {
                    LogrosObtenidos.Clear();
                }

                // 2. Asignamos los textos y la barra de progreso (de 0.0 a 1.0) para la UI
                TextoLogrosFiguras = $"{obtenidosFiguras}/{totalLogrosFiguras}";
                ProgresoLogrosFiguras = totalLogrosFiguras == 0 ? 0 : (double)obtenidosFiguras / totalLogrosFiguras;

                TextoLogrosDiscos = $"{obtenidosDiscos}/{totalLogrosDiscos}";
                ProgresoLogrosDiscos = totalLogrosDiscos == 0 ? 0 : (double)obtenidosDiscos / totalLogrosDiscos;
            }
        }

        private void SetActiveMenu(string menuName)
        {
            if (menuName == "Home")
                return;

            SelectedMenuFigures = menuName == "MyFigures";
            SelectedMenuDiscs = menuName == "MyDiscs";
            SelectedMenuArchi = menuName == "Achievements";
            SelectedMenuSettings = menuName == "Settings";
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
                    { "FiguraActual", figura },
                    { "IsScanFigure",_IsScanFigure }
                };

            var resultado = await _popupPageService.ShowPopupAsync<FiguraInfoPopup, FiguraInfoViewModel, bool>(navParams);

            if(!resultado)
            {
                if (!string.IsNullOrEmpty(figura.CurrentUidHex) && (figura.NfcCodes?.Any()??false))
                {
                    if (figura.NfcCodes.Contains(figura.CurrentUidHex))
                    {
                        figura.NfcCodes.Remove(figura.CurrentUidHex);

                        await Task.Delay(500);

                        await CambiarCantidadAsync(figura, -1);

                        await MostrarAlertaAsync(FontAwesomeIcons.Trash, AppResource.DeleteFigure, Colors.Gray);
                    }
                }
                
            }
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

                _fullListFigures = query.ToList();

                // 2. Filtrar por menú ACTIVO
                if (SelectedMenuFigures)
                    query = query.Where(w => w.Tipo == "Figura");
                else if (SelectedMenuDiscs)
                    query = query.Where(w => w.Tipo == "Disco de Poder");

                var listaFiltrada = query.ToList();

                // 3. Traducir SOLO los que quedaron en el filtro
                foreach (var figura in _fullListFigures)
                {
                    // Al modificar aquí, ya no afectas la consulta original
                    figura.Tipo = figura.Tipo.ToCurrentLanguageTraslate();
                    figura.Franquicia = figura.Franquicia.ToCurrentLanguageTraslate();
                }
                //foreach (var figura in listaFiltrada)
                //{
                //    // Al modificar aquí, ya no afectas la consulta original
                //    figura.Tipo = figura.Tipo.ToCurrentLanguageTraslate();
                //    figura.Franquicia = figura.Franquicia.ToCurrentLanguageTraslate();
                //}

                // 4. Ordenar al final
                _allFigures = (Settings.LanguageSettings == "es")
                    ? listaFiltrada.OrderByDescending(x => x.Tipo).ThenBy(x => x.Version).ThenBy(x => x.Franquicia).ThenBy(x => x.Nombre).ToList()
                    : listaFiltrada.OrderBy(x => x.Tipo).ThenBy(x => x.Version).ThenBy(x => x.Franquicia).ThenBy(x => x.Nombre).ToList();

                // 5. Cargar progreso del usuario
                _userData = await _jsonDataService.ReadUserDataAsync<Dictionary<string, FiguraUserData>>(UserDataFileName);

                foreach (var figura in _fullListFigures)
                {
                    if (_userData.TryGetValue(figura.Id, out var datosUsuario))
                    {
                        figura.Obtenido = datosUsuario.Obtenido;
                        figura.Cantidad = datosUsuario.Cantidad;
                        figura.NfcCodes = new ObservableCollection<string>(datosUsuario.NfcCodes ?? new List<string>());
                    }
                }

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

            if (OpcionesFranquicia.FirstOrDefault(f=>f==FiltroFranquicia)==null)
            {
                FiltroFranquicia = AppResource.All;
            }
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

            IsBusy = true;

            try
            {
                var figuraEnLista = _fullListFigures.FirstOrDefault(f => f.Id == figura.Id);
                if (figuraEnLista is null) return; // Si se sale aquí, el finally apaga el IsBusy mágicamente

                int nuevaCantidad = figuraEnLista.Cantidad + delta;
                if (nuevaCantidad < 0) nuevaCantidad = 0;

                var refrescar = false;
                if (FiltroObtenido == AppResource.Owned && nuevaCantidad == 0) refrescar = true;
                if (FiltroObtenido == AppResource.Owned && nuevaCantidad == 1 && _IsScanFigure) refrescar = true;
                if (FiltroObtenido == AppResource.Missing && nuevaCantidad > 0) refrescar = true;
                if (FiltroObtenido == AppResource.Missing && nuevaCantidad == 0 && _IsScanFigure) refrescar = true;

                figuraEnLista.Cantidad = nuevaCantidad;
                figuraEnLista.Obtenido = nuevaCantidad > 0;

                await GuardarProgresoAsync(figuraEnLista);

                // 💥 AQUI METEMOS EL MOTOR DE LOGROS 💥
                await EvaluarLogros();
                // 💥 FIN DEL MOTOR DE LOGROS 💥

                if (refrescar)
                {
                    AplicarFiltros();
                }
                else
                {
                    ActualizarEstadisticas(ObtenerListaBaseFiltrada());
                }
            }
            finally
            {
                // Esto siempre se va a ejecutar, protegiendo tu app de quedarse pasmada
                IsBusy = false;
            }
        }

        private async Task EvaluarLogros()
        {
            var nuevosLogros = await _logrosService.EvaluarLogrosAsync(_fullListFigures);

            foreach (var logro in nuevosLogros)
            {
                var navParams = new NavigationParameters { { "Logro", logro } };
                await _popupPageService.ShowPopupAsync<LogroDesbloqueadoPopup, LogroDesbloqueadoViewModel, bool>(navParams);

                await Task.Delay(500);
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