using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mopups.Services;

namespace MyDICollection.ViewModels
{
    public partial class AlertMessagePopupViewModel : PopupPageViewModelBase<bool>
    {
        [ObservableProperty]
        private string _icono = string.Empty;

        [ObservableProperty]
        private string _mensaje = string.Empty;

        [ObservableProperty]
        private Color _fontColor = Colors.Gray;
        public AlertMessagePopupViewModel()
        {
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