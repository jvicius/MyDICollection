using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Converters;

namespace MyDICollection.Helpers.Responsive
{
    [ContentProperty(nameof(BaseValue))]
    public class ResponsiveRoundRectangleExtension : IMarkupExtension<RoundRectangle>
    {
        public string BaseValue { get; set; }

        public ResponsiveRoundRectangleExtension()
        {
        }

        public RoundRectangle ProvideValue(IServiceProvider serviceProvider)
        {
            var roundRectangle = new RoundRectangle();

            if (string.IsNullOrEmpty(BaseValue))
                return roundRectangle;

            var converter = new CornerRadiusTypeConverter();
            var radius = (CornerRadius)converter.ConvertFromInvariantString(BaseValue);

            if (DeviceInfo.Current.Idiom == DeviceIdiom.Tablet ||
                DeviceInfo.Current.Idiom == DeviceIdiom.Desktop)
            {
                roundRectangle.CornerRadius = new CornerRadius(
                    (radius.TopLeft / 2.0) * 3,
                    (radius.TopRight / 2.0) * 3,
                    (radius.BottomLeft / 2.0) * 3,
                    (radius.BottomRight / 2.0) * 3
                );
            }
            else
            {
                roundRectangle.CornerRadius = radius;
            }

            return roundRectangle;
        }

        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
        {
            return ProvideValue(serviceProvider);
        }
    }
}
