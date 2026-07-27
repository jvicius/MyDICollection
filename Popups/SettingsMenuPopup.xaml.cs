using Mopups.Pages;
using MyDICollection.ViewModels;

namespace MyDICollection.Popups;

public partial class SettingsMenuPopup : PopupPage
{
	public SettingsMenuPopup()
	{
		InitializeComponent();
	}

    protected override bool OnBackgroundClicked()
    {
        if (BindingContext is SettingsMenuViewModel vm)
        {
            // Devolvemos string vacío para destrabar el proceso
            vm.ResultSource.TrySetResult(string.Empty);
        }
        return base.OnBackgroundClicked();
    }
}