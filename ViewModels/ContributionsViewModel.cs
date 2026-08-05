using CommunityToolkit.Mvvm.Input;
using Mopups.Services;

namespace MyDICollection.ViewModels
{
    public partial class ContributionsViewModel : PopupPageViewModelBase<bool>
    {
        [RelayCommand]
        private async Task AbrirGitHubSponsorsAsync()
        {
            // Abre el enlace en el navegador por defecto del teléfono
            await Launcher.Default.OpenAsync("https://github.com/sponsors/jvicius");
        }

        [RelayCommand]
        private async Task AbrirKoFiAsync()
        {
            await Launcher.Default.OpenAsync("https://ko-fi.com/josevelarde");
        }

        [RelayCommand]
        private async Task CerrarAsync()
        {
            ResultSource.TrySetResult(true);

            if (MopupService.Instance.PopupStack.Count > 0)
            {
                await MopupService.Instance.PopAsync();
            }
        }
    }
}