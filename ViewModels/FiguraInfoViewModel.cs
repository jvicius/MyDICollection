using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mopups.Services;
using MyDICollection.Models;

namespace MyDICollection.ViewModels
{
    public partial class FiguraInfoViewModel : PopupPageViewModelBase<bool>
    {
        [ObservableProperty]
        private FiguraModel _figuraActual;
        [ObservableProperty]

        private bool _isScanFigure;

        [RelayCommand]
        private async Task CerrarAsync()
        {
            // 1. Le decimos al servicio que devuelva "true" (o lo que quieras)
            ResultSource.TrySetResult(true);

            // 2. Cerramos el popup visualmente usando Mopups
            if (MopupService.Instance.PopupStack.Count > 0)
            {
                await MopupService.Instance.PopAsync();
            }
        }


        [RelayCommand]
        private async Task EliminarFiguraAsync()
        {
            // 1. Le decimos al servicio que devuelva "true" (o lo que quieras)
            ResultSource.TrySetResult(false);

            // 2. Cerramos el popup visualmente usando Mopups
            if (MopupService.Instance.PopupStack.Count > 0)
            {
                await MopupService.Instance.PopAsync();
            }
        }
    }
}
