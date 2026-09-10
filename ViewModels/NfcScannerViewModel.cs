using CommunityToolkit.Mvvm.Input;
using Mopups.Services;
using MyDICollection.Helpers.Crypto;
using MyDICollection.Models;
using MyDICollection.Services.Nfc;

namespace MyDICollection.ViewModels
{
    public partial class NfcScannerViewModel : PopupPageViewModelBase<NfcScanResult>
    {
        private readonly IDisneyNfcService _disneyNfcService;
        public NfcScannerViewModel(IDisneyNfcService disneyNfcService) 
        {
            _disneyNfcService = disneyNfcService;

            _disneyNfcService.StartListening();

            _disneyNfcService.ErrorOccurred += _disneyNfcService_OnError;
            _disneyNfcService.FigureDetected += _disneyNfcService_OnFigureDetected;
        }
        [RelayCommand]
        private async Task CerrarAsync()
        {
            _disneyNfcService.StopListening();

            ResultSource.TrySetResult(new NfcScanResult { nfcScanResultEnum = NfcScanResultEnum.Close});

            if (MopupService.Instance.PopupStack.Count > 0)
            {
                await MopupService.Instance.PopAsync();
            }
        }

        private async void _disneyNfcService_OnError(object? sender, string e)
        {
            Console.WriteLine(e);
            _disneyNfcService.StopListening();

            ResultSource.TrySetResult(new NfcScanResult { nfcScanResultEnum = NfcScanResultEnum.Error, errorMessage = e});

            if (MopupService.Instance.PopupStack.Count > 0)
            {
                await MopupService.Instance.PopAsync();
            }

        }

        private async void _disneyNfcService_OnFigureDetected(object? sender, DisneyNfcUtils.DisneyFigureInfo e)
        {
            Console.WriteLine($"UID: {e.UidHex}");
            Console.WriteLine($"ModelNumber: {e.InfCode}");

            ResultSource.TrySetResult(new NfcScanResult { nfcScanResultEnum = NfcScanResultEnum.Success , disneyFigureInfo = e});

            if (MopupService.Instance.PopupStack.Count > 0)
            {
                await MopupService.Instance.PopAsync();
            }

            
        }
    }
}
