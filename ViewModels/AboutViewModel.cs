using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mopups.Services;

namespace MyDICollection.ViewModels;

public partial class AboutViewModel : PopupPageViewModelBase<bool>
{
    [ObservableProperty]
    private string _appVersion;

    public AboutViewModel()
    {
        // Sacamos la versión directo del proyecto
        AppVersion = AppInfo.Current.VersionString;
    }

    [RelayCommand]
    private async Task AbrirGitHubAsync()
    {
        await Browser.Default.OpenAsync("https://github.com/jvicius/MyDICollection", BrowserLaunchMode.SystemPreferred);
    }

    [RelayCommand]
    private async Task AbrirReleasesAsync()
    {
        await Browser.Default.OpenAsync("https://github.com/jvicius/MyDICollection/releases", BrowserLaunchMode.SystemPreferred);
    }

    [RelayCommand]
    private async Task CerrarAsync()
    {
        // 1. Resolvemos la tarea de la clase base regresando true
        ResultSource.TrySetResult(true);

        // 2. Cerramos el popup
        if (MopupService.Instance.PopupStack.Count > 0)
        {
            await MopupService.Instance.PopAsync();
        }
    }
}