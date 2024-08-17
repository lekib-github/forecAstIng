namespace forecAstIng.ViewModel
{
    [QueryProperty(nameof(Forecast), "Forecast")]
    public partial class MorePageViewModel : BaseViewModel
    {
        public MorePageViewModel()
        {
        }

        // This list holds only one value, namely the forecast for which we want details.
        // This is a .NET MAUI hack to both be able to assign Weather/Stock into TimeSeriesData
        // (I had trouble with it before..) and be able to use the nice template selector instead
        // of making 2 pages for Weather/Stock
        [ObservableProperty]
        List<TimeSeriesData> forecast;
    }
}
