using CommunityToolkit.Mvvm.Input;
using Mopups.Services;

namespace MyDICollection.ViewModels
{
    public partial class LanguageViewModel : PopupPageViewModelBase<string>
    {
        [RelayCommand]
        private async Task SeleccionarIdiomaAsync(string idiomaElegido)
        {
            // Seteamos el resultado ("es" o "en")
            ResultSource.TrySetResult(idiomaElegido);

            if (MopupService.Instance.PopupStack.Count > 0)
            {
                await MopupService.Instance.PopAsync();
            }
        }

        [RelayCommand]
        private async Task CerrarAsync()
        {
            // Si le da a la "X", regresamos un string vacío para no hacer nada
            ResultSource.TrySetResult(string.Empty);

            if (MopupService.Instance.PopupStack.Count > 0)
            {
                await MopupService.Instance.PopAsync();
            }
        }
    }
}