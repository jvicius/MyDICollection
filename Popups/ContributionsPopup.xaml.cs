using Mopups.Pages;
using MyDICollection.ViewModels;

namespace MyDICollection.Popups;

public partial class ContributionsPopup : PopupPage
{
	public ContributionsPopup()
	{
		InitializeComponent();
	}

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        if (BindingContext is ContributionsViewModel vm)
        {
            vm.ResultSource.TrySetResult(true);
        }
    }
}