namespace MyDICollection.Controls
{
    public class SafeAreaView : ContentView
    {
        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();
            ApplySafeAreaInsets();
        }

        private void ApplySafeAreaInsets()
        {
#if ANDROID || IOS || WINDOWS
            var insets = GetPlatformSafeArea();
            Padding = insets;
#else
            Padding = new Thickness(0);
#endif
        }

#if ANDROID
        private Thickness GetPlatformSafeArea()
        {
            var context = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity?.Window?.DecorView?.RootWindowInsets;
            if (context == null)
                return new Thickness(0);

            var left = context.StableInsetLeft / DeviceDisplay.MainDisplayInfo.Density;
            var top = context.StableInsetTop / DeviceDisplay.MainDisplayInfo.Density;
            var right = context.StableInsetRight / DeviceDisplay.MainDisplayInfo.Density;
            var bottom = context.StableInsetBottom / DeviceDisplay.MainDisplayInfo.Density;

            return new Thickness(left, top, right, bottom);
        }
#elif IOS
        private Thickness GetPlatformSafeArea()
        {
            var window = UIKit.UIApplication.SharedApplication.KeyWindow;
            if (window == null)
                return new Thickness(0);

            var insets = window.SafeAreaInsets;
            return new Thickness(insets.Left, insets.Top, insets.Right, insets.Bottom);
        }
#elif WINDOWS
        private Thickness GetPlatformSafeArea()
        {
            // En Windows, el "Safe Area" superior corresponde a la barra de título nativa.
            // Por estándar de WinUI, los botones de control de ventana miden 32px de alto.
            // Empujamos 32 puntos hacia abajo (Top) para que nada se empalme.
            return new Thickness(0, 32, 0, 0);
        }
#endif
    }
}