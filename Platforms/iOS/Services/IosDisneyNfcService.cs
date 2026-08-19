using MyDICollection.Helpers.Crypto;
using MyDICollection.Services.Nfc;

namespace MyDICollection.Platforms.iOS.Services
{
    public class IosDisneyNfcService : IDisneyNfcService
    {
        public bool IsSupported => false;

        public bool IsAvailable => false;

        public bool IsEnabled => false;
        public bool IsListening => false;
        public void StartListening() { }
        public void StopListening() { }

        public event EventHandler<
        DisneyNfcUtils.DisneyFigureInfo>?
        FigureDetected;
        public event EventHandler<string> ErrorOccurred;
    }
}
