using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Nfc;
using Android.OS;
using MyDICollection.Platforms.Android.Services;
using MyDICollection.Services.Nfc;

namespace MyDICollection;

[Activity(
    Theme = "@style/Maui.SplashTheme",
    MainLauncher = true,
    LaunchMode = LaunchMode.SingleTop,
    ConfigurationChanges =
        ConfigChanges.ScreenSize |
        ConfigChanges.Orientation |
        ConfigChanges.UiMode |
        ConfigChanges.ScreenLayout |
        ConfigChanges.SmallestScreenSize |
        ConfigChanges.Density)]
public class MainActivity
    : MauiAppCompatActivity
{
    private NfcAdapter? _nfcAdapter;

    protected override void OnCreate(
        Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        _nfcAdapter =
            NfcAdapter.GetDefaultAdapter(
                this);
    }

    protected override void OnResume()
    {
        base.OnResume();

        EnableForegroundNfcDispatch();
    }

    protected override void OnPause()
    {
        DisableForegroundNfcDispatch();

        base.OnPause();
    }

    protected override void OnNewIntent(
        Intent? intent)
    {
        base.OnNewIntent(intent);

        if (intent == null)
            return;

        if (!IsNfcIntent(intent))
            return;

        AndroidDisneyNfcService? service =
            GetDisneyNfcService();

        if (service == null ||
            !service.IsListening)
        {
            // Consumimos silenciosamente el Intent NFC.
            //
            // ForegroundDispatch permanece activo para evitar
            // que Android muestre la UI predeterminada del tag.
            return;
        }

        Tag? tag =
            intent.GetParcelableExtra(
                NfcAdapter.ExtraTag)
            as Tag;

        if (tag == null)
            return;

        _ = service.ProcessTagAsync(tag);
    }

    private AndroidDisneyNfcService?
        GetDisneyNfcService()
    {
        return
            IPlatformApplication.Current?
                .Services
                .GetService<IDisneyNfcService>()
            as AndroidDisneyNfcService;
    }

    private static bool IsNfcIntent(
        Intent intent)
    {
        return
            intent.Action ==
                NfcAdapter.ActionTagDiscovered ||

            intent.Action ==
                NfcAdapter.ActionTechDiscovered ||

            intent.Action ==
                NfcAdapter.ActionNdefDiscovered;
    }

    private void EnableForegroundNfcDispatch()
    {
        if (_nfcAdapter == null)
            return;

        Intent intent =
            new Intent(
                this,
                GetType());

        intent.AddFlags(
            ActivityFlags.SingleTop);

        PendingIntentFlags flags =
            PendingIntentFlags.UpdateCurrent;

        if (Build.VERSION.SdkInt >=
            BuildVersionCodes.S)
        {
            flags |=
                PendingIntentFlags.Mutable;
        }

        PendingIntent pendingIntent =
            PendingIntent.GetActivity(
                this,
                0,
                intent,
                flags);

        _nfcAdapter.EnableForegroundDispatch(
            this,
            pendingIntent,
            null,
            null);
    }

    private void DisableForegroundNfcDispatch()
    {
        if (_nfcAdapter == null)
            return;

        try
        {
            _nfcAdapter
                .DisableForegroundDispatch(
                    this);
        }
        catch
        {
            // Activity ya estaba pausada/detached.
        }
    }
}