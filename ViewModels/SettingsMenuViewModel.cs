using CommunityToolkit.Mvvm.Input;
using Mopups.Services;

namespace MyDICollection.ViewModels
{
    public partial class SettingsMenuViewModel : PopupPageViewModelBase<string>
    {
        [RelayCommand]
        private async Task SeleccionarOpcionAsync(string opcionElegida)
        {
            // 1. Seteamos el resultado usando el ResultSource que viene de tu clase base
            ResultSource.TrySetResult(opcionElegida);

            // 2. Cerramos el popup
            if (MopupService.Instance.PopupStack.Count > 0)
            {
                await MopupService.Instance.PopAsync();
            }
        }
    }
}
