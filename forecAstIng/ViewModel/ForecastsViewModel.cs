using forecAstIng.View;

namespace forecAstIng.ViewModel
{
    public partial class ForecastsViewModel : BaseViewModel
    {
        public ObservableCollection<TimeSeriesData> forecasts { get; } = new();

        public ForecastsViewModel()
        {
            Title = "forecAstIng";
            forecasts.Add(new WeatherData { Name = "Prague, CZ", CurrentBehaviour = "sunny", CurrentValue = 23.8776575, HistoryHigh = 27.389, HistoryLow = 20, MeasurementUnit = 'C' });
            forecasts.Add(new WeatherData { Name = "Banjaluka, BA", CurrentBehaviour = "cloudy", CurrentValue = 25, HistoryHigh = 27.489, HistoryLow = 22, MeasurementUnit = 'C' });
            forecasts.Add(new StockData { Name = "NASDAQ", CurrentBehaviour = "lightning", CurrentValue = 58238, HistoryHigh = 60000, HistoryLow = 2, MeasurementUnit = '$' });
            forecasts.Add(new WeatherData { Name = "Prague, CZ", CurrentBehaviour = "sunny", CurrentValue = 23.8776575, HistoryHigh = 27.389, HistoryLow = 20, MeasurementUnit = 'C' });
            forecasts.Add(new WeatherData { Name = "Banjaluka, BA", CurrentBehaviour = "cloudy", CurrentValue = 25, HistoryHigh = 27.489, HistoryLow = 22, MeasurementUnit = 'C' });
            forecasts.Add(new StockData { Name = "NASDAQ", CurrentBehaviour = "lightning", CurrentValue = 58238, HistoryHigh = 60000, HistoryLow = 2, MeasurementUnit = '$' });
            forecasts.Add(new WeatherData { Name = "Prague, CZ", CurrentBehaviour = "sunny", CurrentValue = 23.8776575, HistoryHigh = 27.389, HistoryLow = 20, MeasurementUnit = 'C' });
            forecasts.Add(new WeatherData { Name = "Banjaluka, BA", CurrentBehaviour = "cloudy", CurrentValue = 25, HistoryHigh = 27.489, HistoryLow = 22, MeasurementUnit = 'C' });
            forecasts.Add(new StockData { Name = "NASDAQ", CurrentBehaviour = "lightning", CurrentValue = 58238, HistoryHigh = 60000, HistoryLow = 2, MeasurementUnit = '$' });
        }

        [RelayCommand]
        async Task GoToMorePage(TimeSeriesData forecast)
        {
            if (forecast is null)
                return;

            await Shell.Current.GoToAsync($"{nameof(MorePage)}", true, new Dictionary<string, object>
                {
                    {"Forecast", forecast }
                });
        }
    }
}
