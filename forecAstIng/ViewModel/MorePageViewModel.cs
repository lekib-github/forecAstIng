namespace forecAstIng.ViewModel
{
    [QueryProperty(nameof(TimeSeriesData), "Forecast")]
    public partial class MorePageViewModel : BaseViewModel
    {
        public MorePageViewModel()
        {
            Title = "More";
        }

        [ObservableProperty]
        public TimeSeriesData forecast;
    } 
}
