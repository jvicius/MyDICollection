using Mopups.Pages;
using MyDICollection.ViewModels;

namespace MyDICollection.Popups;

public partial class FiguraInfoPopup : PopupPage
{
    public FiguraInfoPopup()
    {
        InitializeComponent();
    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        if (BindingContext is FiguraInfoViewModel vm)
        {
            vm.ResultSource.TrySetResult(true);
        }
    }
}