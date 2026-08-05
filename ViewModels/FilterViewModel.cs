using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mopups.Services;
using MyDICollection.Models;
using MyDICollection.Resources;

namespace MyDICollection.ViewModels
{
    public partial class FilterViewModel : PopupPageViewModelBase<FilterParams>
    {
        [ObservableProperty]
        private FilterParams _filtros;

        // Sobrescribimos el método de inicialización (si tu clase base lo permite) 
        // o simplemente le pasas el parámetro al construirlo/navegar.
        public void CargarFiltros(FilterParams parametrosActuales)
        {
            Filtros = parametrosActuales;
        }

        [RelayCommand]
        private async Task LimpiarFiltrosAsync()
        {
            Filtros.FiltroObtenido = AppResource.All;
            Filtros.FiltroVersion = AppResource.All;
            Filtros.FiltroFranquicia = AppResource.All;

            // Forzamos la actualización en la UI
            OnPropertyChanged(nameof(Filtros));
        }

        [RelayCommand]
        private async Task AplicarAsync()
        {
            ResultSource.TrySetResult(Filtros);

            if (MopupService.Instance.PopupStack.Count > 0)
            {
                await MopupService.Instance.PopAsync();
            }
        }

        [RelayCommand]
        private async Task CerrarAsync()
        {
            // Regresamos null para indicar que canceló
            ResultSource.TrySetResult(null);

            if (MopupService.Instance.PopupStack.Count > 0)
            {
                await MopupService.Instance.PopAsync();
            }
        }
    }
}