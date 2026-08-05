using System.Windows.Input;

namespace MyDICollection.Controls;

public partial class MenuTabItem : ContentView
{
    public MenuTabItem()
    {
        InitializeComponent();
        // Establecer color inicial
        UpdateColor();
    }

    // 1. Texto del Icono
    public static readonly BindableProperty IconTextProperty = BindableProperty.Create(nameof(IconText), typeof(string), typeof(MenuTabItem), string.Empty);
    public string IconText { get => (string)GetValue(IconTextProperty); set => SetValue(IconTextProperty, value); }

    // 2. Texto del Label
    public static readonly BindableProperty LabelTextProperty = BindableProperty.Create(nameof(LabelText), typeof(string), typeof(MenuTabItem), string.Empty);
    public string LabelText { get => (string)GetValue(LabelTextProperty); set => SetValue(LabelTextProperty, value); }

    // 3. Comando
    public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(MenuTabItem), null);
    public ICommand Command { get => (ICommand)GetValue(CommandProperty); set => SetValue(CommandProperty, value); }

    // 4. Parámetro del Comando
    public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(MenuTabItem), null);
    public object CommandParameter { get => GetValue(CommandParameterProperty); set => SetValue(CommandParameterProperty, value); }

    // 5. Estado de Selección (Activo/Inactivo)
    public static readonly BindableProperty IsSelectedProperty = BindableProperty.Create(nameof(IsSelected), typeof(bool), typeof(MenuTabItem), false, propertyChanged: OnStateChanged);
    public bool IsSelected { get => (bool)GetValue(IsSelectedProperty); set => SetValue(IsSelectedProperty, value); }

    // 6. Color Seleccionado
    public static readonly BindableProperty SelectedColorProperty = BindableProperty.Create(nameof(SelectedColor), typeof(Color), typeof(MenuTabItem), Colors.Red, propertyChanged: OnStateChanged);
    public Color SelectedColor { get => (Color)GetValue(SelectedColorProperty); set => SetValue(SelectedColorProperty, value); }

    // 7. Color No Seleccionado
    public static readonly BindableProperty UnselectedColorProperty = BindableProperty.Create(nameof(UnselectedColor), typeof(Color), typeof(MenuTabItem), Colors.Gray, propertyChanged: OnStateChanged);
    public Color UnselectedColor { get => (Color)GetValue(UnselectedColorProperty); set => SetValue(UnselectedColorProperty, value); }

    // Propiedad interna para que el XAML sepa qué color pintar
    public static readonly BindableProperty CurrentColorProperty = BindableProperty.Create(nameof(CurrentColor), typeof(Color), typeof(MenuTabItem), Colors.Gray);
    public Color CurrentColor { get => (Color)GetValue(CurrentColorProperty); private set => SetValue(CurrentColorProperty, value); }

    private static void OnStateChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (MenuTabItem)bindable;
        control.UpdateColor();
    }

    private void UpdateColor()
    {
        CurrentColor = IsSelected ? SelectedColor : UnselectedColor;
    }

    private async void OnControlTapped(object sender, TappedEventArgs e)
    {
        // 1. Animación de apachurre (Encoge a 0.85 y dura 100 milisegundos)
        await thisControl.ScaleTo(0.85, 100, Easing.CubicOut);

        // 2. Regresa a la normalidad (Escala 1.0)
        await thisControl.ScaleTo(1.0, 100, Easing.CubicIn);

        // 3. Ya que terminó la animación, ejecutamos el comando (si es que le bindeaste uno)
        if (Command != null && Command.CanExecute(CommandParameter))
        {
            Command.Execute(CommandParameter);
        }
    }
}