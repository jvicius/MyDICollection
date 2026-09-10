using MyDICollection.Helpers.Crypto;

namespace MyDICollection.Models
{
    public enum NfcScanResultEnum 
    { 
        Success,
        Close,
        Error
    }
    public class NfcScanResult
    {
        public DisneyNfcUtils.DisneyFigureInfo disneyFigureInfo { set; get; }
        public NfcScanResultEnum nfcScanResultEnum { set; get; } = NfcScanResultEnum.Success;
        public string errorMessage { set; get; }

    }
}
