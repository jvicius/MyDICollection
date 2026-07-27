using Mopups.Pages;
using MyDICollection.Models;

namespace MyDICollection.Services
{
    public interface IPopupPageService
    {
        Task<T> ShowPopupAsync<TPage, TViewModel, T>(INavigationParameters? parameters = null)
        where TPage : PopupPage
        where TViewModel : class, IPopupResultViewModel<T>;
    }
}
