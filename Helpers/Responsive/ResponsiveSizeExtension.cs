namespace MyDICollection.Helpers.Responsive
{
    [ContentProperty(nameof(BaseValue))]
    public class ResponsiveSizeExtension : IMarkupExtension<double>
    {
        public double BaseValue { get; set; }

        // 1. Agrega este constructor vacío (obligatorio para XAML)
        public ResponsiveSizeExtension()
        {
        }

        // 2. Agrega este constructor que recibe el valor directamente
        public ResponsiveSizeExtension(double baseValue)
        {
            BaseValue = baseValue;
        }

        public double ProvideValue(IServiceProvider serviceProvider)
        {
            if (DeviceInfo.Current.Idiom == DeviceIdiom.Phone)
            {
                return BaseValue;

            }
            return (BaseValue / 2) * 3;
        }

        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
        {
            return ProvideValue(serviceProvider);
        }
    }
}
