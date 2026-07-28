using Mopups.Pages;
using MyDICollection.ViewModels;

namespace MyDICollection.Popups;

public partial class LanguagePopup : PopupPage
{
	public LanguagePopup()
	{
		InitializeComponent();
	}

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        if (BindingContext is LanguageViewModel vm)
        {
            // Evita bloqueos si el usuario cierra tocando el fondo oscuro
            vm.ResultSource.TrySetResult(string.Empty);
        }
    }
}