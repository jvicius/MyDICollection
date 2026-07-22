using Microsoft.Maui.Converters;

namespace MyDICollection.Helpers.Responsive
{
    [ContentProperty(nameof(BaseValue))]
    public class ResponsiveThicknessExtension : IMarkupExtension<Thickness>
    {
        public string BaseValue { get; set; }

        public ResponsiveThicknessExtension()
        {
        }

        public Thickness ProvideValue(IServiceProvider serviceProvider)
        {
            if (string.IsNullOrEmpty(BaseValue))
                return new Thickness(0);

            var converter = new ThicknessTypeConverter();
            var thickness = (Thickness)converter.ConvertFromInvariantString(BaseValue);

            if (DeviceInfo.Current.Idiom == DeviceIdiom.Phone)
            {
                return thickness;
            }

            return new Thickness(
                    (thickness.Left / 2.0) * 3,
                    (thickness.Top / 2.0) * 3,
                    (thickness.Right / 2.0) * 3,
                    (thickness.Bottom / 2.0) * 3
                );
        }

        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
        {
            return ProvideValue(serviceProvider);
        }
    }
}
