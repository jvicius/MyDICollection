using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using Mopups.Hosting;
using MyDICollection.Helpers;
using MyDICollection.Popups;
using MyDICollection.Services;
using MyDICollection.ViewModels;
using System.Globalization;

namespace MyDICollection
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            InitConfig(); 

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseMauiCommunityToolkitCore()
                .ConfigureMopups()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("fa-solid-900.otf", "FASolid");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton<IPopupPageService, PopupPageService>();
            builder.Services.AddSingleton<IJsonDataService, JsonDataService>();
            builder.Services.AddSingleton<ILocalizationService, LocalizationService>();
            builder.Services.AddSingleton<StatusBarService>();

            builder.Services.AddTransient<FiguraInfoPopup>();
            builder.Services.AddTransient<FiguraInfoViewModel>();
            builder.Services.AddTransient<AboutPopup>();
            builder.Services.AddTransient<AboutViewModel>();
            builder.Services.AddTransient<LanguagePopup>();
            builder.Services.AddTransient<LanguageViewModel>();
            builder.Services.AddTransient<ContributionsPopup>();
            builder.Services.AddTransient<ContributionsViewModel>();
            builder.Services.AddTransient<FilterPopup>();
            builder.Services.AddTransient<FilterViewModel>();

            builder.Services.AddTransient<MainPageViewModel>();
            builder.Services.AddTransient<MainPage>();

            builder.ConfigureLifecycleEvents(events =>
            {
#if WINDOWS
    events.AddWindows(windows => windows
        .OnWindowCreated(window =>
        {
            // Obtenemos el "Handle" (identificador nativo) de la ventana de Windows
            var handle = WinRT.Interop.WindowNative.GetWindowHandle(window);
            var id = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(handle);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(id);

            // Sacamos el Presenter, que es el que controla si está en pantalla completa, minimizada, etc.
            var presenter = appWindow.Presenter as Microsoft.UI.Windowing.OverlappedPresenter;
            if (presenter != null)
            {
                // ¡Maximizamos alv!
                presenter.Maximize();
            }
        }));
#endif
            });

            return builder.Build();
        }

        private static void InitConfig()
        {
            SetupLanguage();
        }
        private static void SetupLanguage()
        {
            //force language
            //Settings.LanguageSettings = "en";

            if (string.IsNullOrEmpty(Settings.LanguageSettings))
                Settings.LanguageSettings = CultureInfo.CurrentCulture.TwoLetterISOLanguageName.ToLower();

            var LocalizationService = new LocalizationService();
            LocalizationService.SetCulture(Settings.LanguageSettings);
        }
    }
}
