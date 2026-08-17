using Android.Nfc;
using Android.Nfc.Tech;
using MyDICollection.Helpers.Crypto;
using MyDICollection.Services.Nfc;

namespace MyDICollection.Platforms.Android.Services;

public sealed class AndroidDisneyNfcService
    : IDisneyNfcService
{
    private readonly SemaphoreSlim _readLock =
        new(1, 1);

    public bool IsSupported =>
        true;

    public bool IsAvailable
    {
        get
        {
            NfcAdapter? adapter =
                NfcAdapter.GetDefaultAdapter(
                    global::Android.App.Application.Context);

            return adapter != null;
        }
    }

    public bool IsEnabled
    {
        get
        {
            NfcAdapter? adapter =
                NfcAdapter.GetDefaultAdapter(
                    global::Android.App.Application.Context);

            return adapter?.IsEnabled == true;
        }
    }

    public bool IsListening { get; private set; }

    public event EventHandler<
        DisneyNfcUtils.DisneyFigureInfo>?
        FigureDetected;

    public event EventHandler<string>?
        ErrorOccurred;

    public void StartListening()
    {
        if (!IsAvailable)
        {
            RaiseError(
                "Este dispositivo no cuenta con NFC.");

            return;
        }

        if (!IsEnabled)
        {
            RaiseError(
                "NFC está deshabilitado.");

            return;
        }

        IsListening = true;
    }

    public void StopListening()
    {
        IsListening = false;
    }

    /// <summary>
    /// Android-specific entry point called by MainActivity.
    /// </summary>
    public async Task ProcessTagAsync(
        Tag tag,
        CancellationToken cancellationToken = default)
    {
        if (!IsListening)
            return;

        // Evita procesar dos intents NFC simultáneamente.
        if (!await _readLock.WaitAsync(
                0,
                cancellationToken))
        {
            return;
        }

        try
        {
            DisneyNfcUtils.DisneyFigureInfo? figure =
                await Task.Run(
                    () => ReadFigure(tag),
                    cancellationToken);

            if (figure == null)
            {
                RaiseError(
                    "No fue posible identificar la figura.");

                return;
            }

            if (!figure.IsChecksumValid)
            {
                RaiseError(
                    "La información NFC de la figura no superó la validación de integridad.");

                return;
            }

            FigureDetected?.Invoke(
                this,
                figure);
        }
        catch (OperationCanceledException)
        {
            // Cancelación esperada.
        }
        catch (Exception ex)
        {
            RaiseError(
                $"Error leyendo la figura: {ex.Message}");
        }
        finally
        {
            StopListening();

            _readLock.Release();
        }
    }

    private static DisneyNfcUtils.DisneyFigureInfo?
        ReadFigure(Tag tag)
    {
        byte[] uid =
            tag.GetId();

        byte[] mifareKey =
            DisneyNfcUtils.CalculateMifareKey(
                uid);

        MifareClassic? mifare =
            MifareClassic.Get(tag);

        if (mifare == null)
        {
            throw new NotSupportedException(
                "El dispositivo no reportó soporte MIFARE Classic para esta figura.");
        }

        try
        {
            mifare.Connect();

            const int identificationSector = 0;

            bool authenticated =
                mifare.AuthenticateSectorWithKeyA(
                    identificationSector,
                    mifareKey);

            if (!authenticated)
            {
                authenticated =
                    mifare.AuthenticateSectorWithKeyB(
                        identificationSector,
                        mifareKey);
            }

            if (!authenticated)
            {
                throw new InvalidOperationException(
                    "No fue posible autenticar el sector de identificación.");
            }

            int firstBlock =
                mifare.SectorToBlock(
                    identificationSector);

            // Sector 0:
            //
            // B00 = manufacturing
            // B01 = identification data
            // B02 = additional data
            // B03 = trailer
            const int identificationBlockOffset = 1;

            int blockNumber =
                firstBlock +
                identificationBlockOffset;

            byte[] encryptedBlock1 =
                mifare.ReadBlock(
                    blockNumber);

#if DEBUG
            global::Android.Util.Log.Debug(
                "DISNEY_NFC",
                $"UID     : {DisneyNfcUtils.ToHex(uid)}");

            global::Android.Util.Log.Debug(
                "DISNEY_NFC",
                $"B01 ENC : {DisneyNfcUtils.ToHex(encryptedBlock1)}");
#endif

            DisneyNfcUtils.DisneyFigureInfo? figure =
                DisneyNfcUtils.TryGetFigureInfoFromBlock1(
                    encryptedBlock1,
                    uid);

#if DEBUG
            if (figure != null)
            {
                global::Android.Util.Log.Debug(
                    "DISNEY_NFC",
                    $"MODEL   : {figure.InfCode}");

                global::Android.Util.Log.Debug(
                    "DISNEY_NFC",
                    $"CRC     : {(figure.IsChecksumValid ? "OK" : "FAIL")}");

                global::Android.Util.Log.Debug(
                    "DISNEY_NFC",
                    $"B01 DEC : {DisneyNfcUtils.ToHex(figure.DecryptedBlock1)}");
            }
#endif

            return figure;
        }
        finally
        {
            try
            {
                if (mifare.IsConnected)
                {
                    mifare.Close();
                }
            }
            catch
            {
                // El tag pudo retirarse antes de cerrar.
            }
        }
    }

    private void RaiseError(
        string message)
    {
        ErrorOccurred?.Invoke(
            this,
            message);
    }
}