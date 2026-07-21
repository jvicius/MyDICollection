namespace MyDICollection.Helpers.Responsive
{
    [ContentProperty(nameof(BaseValue))]
    public class ResponsiveIntExtension : IMarkupExtension<int>
    {
        public int BaseValue { get; set; }

        public ResponsiveIntExtension() { }

        public int ProvideValue(IServiceProvider serviceProvider)
        {

            if (DeviceInfo.Current.Idiom == DeviceIdiom.Phone)
            {
                return BaseValue;

            }

            return (int)Math.Round((BaseValue / 2.0) * 3);
        }

        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
        {
            return ProvideValue(serviceProvider);
        }
    }
}
