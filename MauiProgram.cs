using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core;
using Microsoft.Extensions.Logging;
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
            builder.Services.AddTransient<SettingsMenuPopup>();
            builder.Services.AddTransient<SettingsMenuViewModel>();
            builder.Services.AddTransient<AboutPopup>();
            builder.Services.AddTransient<AboutViewModel>();
            builder.Services.AddTransient<LanguagePopup>();
            builder.Services.AddTransient<LanguageViewModel>();

            builder.Services.AddTransient<MainPageViewModel>();
            builder.Services.AddTransient<MainPage>();

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
