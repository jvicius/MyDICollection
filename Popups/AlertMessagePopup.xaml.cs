using Mopups.Pages;
using MyDICollection.ViewModels;

namespace MyDICollection.Popups;

public partial class AlertMessagePopup : PopupPage
{
	public AlertMessagePopup()
	{
		InitializeComponent();
	}

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        if (BindingContext is AlertMessagePopupViewModel vm)
        {
            // Si lo cerraron picando afuera, destrabamos el hilo mandando un false
            vm.ResultSource.TrySetResult(false);
        }
    }
}