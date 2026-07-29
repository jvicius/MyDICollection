using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mopups.Services;
using MyDICollection.Models;

namespace MyDICollection.ViewModels
{
    public partial class LogroDesbloqueadoViewModel : PopupPageViewModelBase<bool>
    {
        [ObservableProperty]
        private LogroDefinicion _logro;

        public LogroDesbloqueadoViewModel()
        {
            CerrarAutomaticamente();
        }

        private async void CerrarAutomaticamente()
        {
            await Task.Delay(4000);
            await CerrarLogro();
        }

        [RelayCommand]
        private async Task CerrarManualmenteAsync()
        {
            await CerrarLogro();
        }

        private async Task CerrarLogro()
        {
            if (MopupService.Instance.PopupStack.Any(p => p.BindingContext == this))
            {
                ResultSource.TrySetResult(true);
                await MopupService.Instance.PopAsync();
            }
        }
    }
}