using MyDICollection.Helpers.Crypto;

namespace MyDICollection.Services.Nfc;

public interface IDisneyNfcService
{
    /// <summary>
    /// La plataforma tiene una implementación NFC disponible.
    /// </summary>
    bool IsSupported { get; }

    /// <summary>
    /// El dispositivo cuenta con hardware NFC compatible.
    /// </summary>
    bool IsAvailable { get; }

    /// <summary>
    /// El NFC del dispositivo está habilitado.
    /// </summary>
    bool IsEnabled { get; }

    /// <summary>
    /// Indica si actualmente se espera una figura.
    /// </summary>
    bool IsListening { get; }

    /// <summary>
    /// Comienza a esperar una figura NFC.
    /// </summary>
    void StartListening();

    /// <summary>
    /// Deja de esperar una figura NFC.
    /// </summary>
    void StopListening();

    /// <summary>
    /// Se dispara cuando una figura Disney Infinity
    /// ha sido identificada correctamente.
    /// </summary>
    event EventHandler<DisneyNfcUtils.DisneyFigureInfo>? FigureDetected;

    /// <summary>
    /// Se dispara cuando ocurre un error durante
    /// la identificación NFC.
    /// </summary>
    event EventHandler<string>? ErrorOccurred;
}