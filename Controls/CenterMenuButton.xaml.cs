using System.Windows.Input;

namespace MyDICollection.Controls;

public partial class CenterMenuButton : ContentView
{
    public CenterMenuButton()
    {
        InitializeComponent();
    }

    // 1. Imagen (ImageSource)
    public static readonly BindableProperty ImageSourceProperty = BindableProperty.Create(nameof(ImageSource), typeof(ImageSource), typeof(CenterMenuButton), null);
    public ImageSource ImageSource { get => (ImageSource)GetValue(ImageSourceProperty); set => SetValue(ImageSourceProperty, value); }

    // 2. Comando
    public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(CenterMenuButton), null);
    public ICommand Command { get => (ICommand)GetValue(CommandProperty); set => SetValue(CommandProperty, value); }

    // 3. Parámetro del Comando
    public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(CenterMenuButton), null);
    public object CommandParameter { get => GetValue(CommandParameterProperty); set => SetValue(CommandParameterProperty, value); }

    // Evento de animación y ejecución del comando
    private async void OnCenterButtonTapped(object sender, TappedEventArgs e)
    {
        // 1. Animación de apachurre (Encoge a 0.85 y dura 100 milisegundos)
        await thisControl.ScaleTo(0.85, 100, Easing.CubicOut);

        // 2. Regresa a la normalidad
        await thisControl.ScaleTo(1.0, 100, Easing.CubicIn);

        // 3. Ejecutamos el comando
        if (Command != null && Command.CanExecute(CommandParameter))
        {
            Command.Execute(CommandParameter);
        }
    }
}