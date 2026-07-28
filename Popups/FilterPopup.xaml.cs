using Mopups.Pages;
using MyDICollection.ViewModels;

namespace MyDICollection.Popups;

public partial class FilterPopup : PopupPage
{
	public FilterPopup()
	{
		InitializeComponent();
	}

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        if (BindingContext is FilterViewModel vm)
        {
            // Si lo cerraron picando afuera, destrabamos el hilo mandando un false
            vm.ResultSource.TrySetResult(null);
        }
    }
}