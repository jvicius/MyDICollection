using CommunityToolkit.Mvvm.ComponentModel;
using MyDICollection.Services;

namespace MyDICollection.ViewModels
{
    public partial class PopupPageViewModelBase<T> : ObservableObject, IPopupResultViewModel<T>
    {
        public TaskCompletionSource<T> ResultSource { get; set; } = new();
    }
}
