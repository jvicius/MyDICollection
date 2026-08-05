using Mopups.Pages;
using MyDICollection.ViewModels;

namespace MyDICollection.Popups;

public partial class LogroDesbloqueadoPopup : PopupPage
{
	public LogroDesbloqueadoPopup()
	{
		InitializeComponent();
	}

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        if (BindingContext is LogroDesbloqueadoViewModel vm)
        {
            // Si lo cerraron picando afuera, destrabamos el hilo mandando un false
            vm.ResultSource.TrySetResult(false);
        }
    }
}