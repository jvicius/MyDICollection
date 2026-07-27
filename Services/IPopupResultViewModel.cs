namespace MyDICollection.Services
{
    public interface IPopupResultViewModel<T>
    {
        TaskCompletionSource<T> ResultSource { get; set; }
    }
}