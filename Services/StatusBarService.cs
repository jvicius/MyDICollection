using Microsoft.Maui.Platform;
using MauiColor = Microsoft.Maui.Graphics.Color;

#if ANDROID
using Android.Animation;
using Android.Content.Res;
using Android.Graphics;
using Android.Views;
#endif

namespace MyDICollection.Services
{
    public class StatusBarService
    {
        public void SetSystemBars(
            MauiColor? lightStatusBarColor = null, MauiColor? darkStatusBarColor = null,
            MauiColor? lightNavigationBarColor = null, MauiColor? darkNavigationBarColor = null,
            bool animate = true)
        {
#if ANDROID
            var window = Platform.CurrentActivity?.Window;
            if (window == null)
                return;

            bool isDarkMode = (window.Context?.Resources?.Configuration?.UiMode & UiMode.NightMask) == UiMode.NightYes;

            var statusBarColor = isDarkMode ? darkStatusBarColor ?? MauiColor.FromArgb("#000000") : lightStatusBarColor ?? MauiColor.FromArgb("#FFFFFF");
            var navigationBarColor = isDarkMode ? darkNavigationBarColor ?? MauiColor.FromArgb("#000000") : lightNavigationBarColor ?? MauiColor.FromArgb("#FFFFFF");

            //navigationBarColor = MauiColor.FromArgb("#FFFFFF");

            if (animate)
            {
                AnimateColorChange(window, window.StatusBarColor, statusBarColor, isStatusBar: true);
                AnimateColorChange(window, window.NavigationBarColor, navigationBarColor, isStatusBar: false);
            }
            else
            {
                window.SetStatusBarColor(statusBarColor.ToPlatform());
                window.SetNavigationBarColor(navigationBarColor.ToPlatform());
            }

            var decorView = window.DecorView;
            var flags = (StatusBarVisibility)decorView.SystemUiVisibility;

            if (IsColorLight(statusBarColor))
                 flags |= (StatusBarVisibility)SystemUiFlags.LightStatusBar;
            else
                flags &= ~(StatusBarVisibility)SystemUiFlags.LightStatusBar;

            if (IsColorLight(navigationBarColor))
                flags |= (StatusBarVisibility)SystemUiFlags.LightNavigationBar;
            else
                flags &= ~(StatusBarVisibility)SystemUiFlags.LightNavigationBar;

            decorView.SystemUiVisibility = flags;
#endif
        }

#if ANDROID
private static void AnimateColorChange(Android.Views.Window window, int fromColorInt, MauiColor toColor, bool isStatusBar)
{
    var fromColor = new Android.Graphics.Color(fromColorInt);
    var toColorAndroid = new Android.Graphics.Color((int)(toColor.Red * 255), (int)(toColor.Green * 255), (int)(toColor.Blue * 255));

    var animator = ValueAnimator.OfArgb(fromColor.ToArgb(), toColorAndroid.ToArgb());
    animator.SetDuration(300); // Duración de la animación en ms

    animator.Update += (sender, e) =>
    {
        var value = (int)e.Animation.AnimatedValue;
        if (isStatusBar)
            window.SetStatusBarColor(new Android.Graphics.Color(value));
        else
            window.SetNavigationBarColor(new Android.Graphics.Color(value));
    };

    animator.Start();
}
#endif
        private bool IsColorLight(MauiColor color)
        {
            double luminance = (0.299 * color.Red + 0.587 * color.Green + 0.114 * color.Blue);
            return luminance > 0.5;
        }
    }
}
