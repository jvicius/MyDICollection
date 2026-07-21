using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using MyDICollection.Helpers;
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
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton<IJsonDataService, JsonDataService>();
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
            if (string.IsNullOrEmpty(Settings.LanguageSettings))
                Settings.LanguageSettings = CultureInfo.CurrentCulture.TwoLetterISOLanguageName.ToLower();

            var LocalizationService = new LocalizationService();
            LocalizationService.SetCulture(Settings.LanguageSettings);
        }
    }
}
