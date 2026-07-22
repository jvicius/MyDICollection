using System.Windows.Input;

namespace MyDICollection.Controls;

public partial class HexagonButton : ContentView
{
    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(HexagonButton), string.Empty);

    public static readonly BindableProperty TextColorProperty =
        BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(HexagonButton), Colors.White);

    public static readonly BindableProperty HexBackgroundColorProperty =
        BindableProperty.Create(nameof(HexBackgroundColor), typeof(Color), typeof(HexagonButton), Colors.DarkGray);

    public static readonly BindableProperty HexBorderColorProperty =
        BindableProperty.Create(nameof(HexBorderColor), typeof(Color), typeof(HexagonButton), Colors.Orange);

    public static readonly BindableProperty CommandProperty =
        BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(HexagonButton));

    public static readonly BindableProperty CommandParameterProperty =
        BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(HexagonButton));

    public static readonly BindableProperty FontSizeProperty =
       BindableProperty.Create(nameof(FontSize), typeof(double), typeof(HexagonButton), (double) 12);

    public double FontSize
    {
        get => (double)GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }
    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public Color TextColor
    {
        get => (Color)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }

    public Color HexBackgroundColor
    {
        get => (Color)GetValue(HexBackgroundColorProperty);
        set => SetValue(HexBackgroundColorProperty, value);
    }

    public Color HexBorderColor
    {
        get => (Color)GetValue(HexBorderColorProperty);
        set => SetValue(HexBorderColorProperty, value);
    }

    public ICommand Command
    {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public object CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    public HexagonButton()
    {
        InitializeComponent();
    }

    private async void OnTapped(object sender, TappedEventArgs e)
    {
        await this.ScaleTo(0.9, 100, Easing.CubicOut);
        await this.ScaleTo(1.0, 100, Easing.CubicIn);

        if (Command != null && Command.CanExecute(CommandParameter))
        {
            Command.Execute(CommandParameter);
        }
    }
}