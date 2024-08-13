namespace forecAstIng.View
{
    // Everything here is broken </3 weather data/stock data -> timeseries data is an invalid cast? how?
    public partial class MorePage : ContentPage
    {
        public MorePage(MorePageViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }

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
