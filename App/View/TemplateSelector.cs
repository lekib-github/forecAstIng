namespace forecAstIng.View
{
    public class TimeSeriesDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate WeatherDataTemplate { get; set; }
        public DataTemplate StockDataTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            return item switch
            {
                WeatherData => WeatherDataTemplate,
                StockData => StockDataTemplate,
                _ => null
            };
        }
    }
}
