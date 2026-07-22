namespace MyDICollection.Helpers.Responsive
{
    [ContentProperty(nameof(BaseValue))]
    public class ResponsiveSizeExtension : IMarkupExtension<double>
    {
        public double BaseValue { get; set; }

        public ResponsiveSizeExtension()
        {
        }
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
