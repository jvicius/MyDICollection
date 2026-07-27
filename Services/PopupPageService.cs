using Mopups.Pages;
using Mopups.Services;
using MyDICollection.Models;
using System.Reflection;

#if IOS
using UIKit;
using Foundation;
#endif

namespace MyDICollection.Services
{
    public class PopupPageService : IPopupPageService
    {
        private readonly IServiceProvider _serviceProvider;

        public PopupPageService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<T> ShowPopupAsync<TPage, TViewModel, T>(INavigationParameters? parameters = null)
            where TPage : PopupPage
            where TViewModel : class, IPopupResultViewModel<T>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                // 1. Intento genérico de MAUI para quitar foco
                var focusedElement = Shell.Current?.CurrentPage?.GetVisualTreeDescendants()
                                            .OfType<VisualElement>()
                                            .FirstOrDefault(x => x.IsFocused);
                focusedElement?.Unfocus();

                // 2. El "Mata-Teclados" definitivo para .NET 9 iOS
#if IOS
    // Buscamos la escena activa para llegar a la ventana principal
    var window = UIApplication.SharedApplication.ConnectedScenes
        .OfType<UIWindowScene>()
        .SelectMany(s => s.Windows)
        .FirstOrDefault(w => w.IsKeyWindow);

    // EndEditing(true) se llama sobre la View, no directamente sobre el Window en algunas versiones
    window?.EndEditing(true); 
#endif
            });

            // Obtener el Popup y el ViewModel
            var popup = _serviceProvider.GetRequiredService<TPage>();
            var viewModel = _serviceProvider.GetRequiredService<TViewModel>();

            // Si hay parámetros, asignarlos al ViewModel
            if (parameters != null)
            {
                AssignParametersToViewModel(viewModel, parameters);
            }

            // Asignar el ViewModel al BindingContext del Popup
            popup.BindingContext = viewModel;

            // Mostrar el Popup
            await MopupService.Instance.PushAsync(popup);

            // Esperar el resultado del ViewModel
            return await viewModel.ResultSource.Task;
        }

        // Método mejorado para asignar los parámetros al ViewModel
        private void AssignParametersToViewModel(object viewModel, INavigationParameters parameters)
        {
            foreach (var param in parameters)
            {
                // Obtener la propiedad en el ViewModel
                var property = viewModel.GetType().GetProperty(param.Key, BindingFlags.Public | BindingFlags.Instance);
                if (property != null && property.CanWrite)
                {
                    try
                    {
                        // Verificar si el tipo de propiedad y el parámetro coinciden
                        var targetType = property.PropertyType;
                        if (param.Value != null && targetType.IsAssignableFrom(param.Value.GetType()))
                        {
                            property.SetValue(viewModel, param.Value);
                        }
                        else
                        {
                            // Intentar convertir el parámetro al tipo de la propiedad
                            var convertedValue = Convert.ChangeType(param.Value, targetType);
                            property.SetValue(viewModel, convertedValue);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Manejo de error si la conversión o asignación falla
                        Console.WriteLine($"Error al asignar el parámetro {param.Key} al ViewModel: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine($"Propiedad {param.Key} no encontrada o no tiene un setter.");
                }
            }
        }
    }
}
