namespace forecAstIng.ViewModel
{
    [QueryProperty(nameof(Forecast), "Forecast")]
    public partial class MorePageViewModel : BaseViewModel
    {
        public MorePageViewModel()
        {
        }

        [ObservableProperty]
        TimeSeriesData forecast;
    }
}
