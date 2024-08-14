using CommunityToolkit.Maui.Views;
using forecAstIng.Services;
using forecAstIng.View;

namespace forecAstIng.ViewModel
{
    public partial class ForecastsViewModel : BaseViewModel
    {
        Popup? searchPopup;
        public ObservableCollection<TimeSeriesData> Forecasts { get; } = new();
        DataService dataService;

        public ForecastsViewModel(DataService service)
        {
            Title = "forecAstIng";
            dataService = service;

            LoadLastLocationForecast();
        }

        // Will need to change according to WeatherData etc. Even as it stands there is a lot of hardcoding, and it is not complete
        private WeatherData ConstructWeatherData(Geoloc geoloc, Weather weather)
        {
            return new WeatherData { Name = $"{geoloc.address.city}, {geoloc.address.country_code}", 
                                     MeasurementUnit = weather.hourly_units.temperature_2m,
                                     HistoryHigh = weather.daily.temperature_2m_max.Max(),
                                     HistoryLow = weather.daily.temperature_2m_min.Min(),
                                     CurrentValue = weather.hourly.temperature_2m[24*7 + 12]};
        }

        // No granular exception handling; automatic last location forecast adding is a QOL feauture, and the user can attempt
        // to add their location manually if it fails, where they will get more data.
        async Task LoadLastLocationForecast()
        {
            try
            {
                var (geoloc, weather) = await dataService.GetLastLocationForecast();

                Forecasts.Add(ConstructWeatherData(geoloc, weather));
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", "Attempt to add your location to forecasts automatically failed, consider adding it manually.", "OK");
            }
        }

        [RelayCommand]
        async Task GoToMorePage(TimeSeriesData forecast)
        {
            if (forecast is null)
                return;

            await Shell.Current.GoToAsync(nameof(MorePage), true, new Dictionary<string, object>
                {
                    {"Forecast", forecast}
                });
        }

        [RelayCommand]
        void OpenSearchPopup()
        {
            searchPopup = new SearchPrompt();
            Shell.Current.ShowPopup(searchPopup);
        }

        [RelayCommand]
        async Task AddRequested(string requestedName)
        {
            if (IsWorking)
            {
                return;
            }

            try
            {
                IsWorking = true;
                NetworkAccess accessType = Connectivity.Current.NetworkAccess;

                if (accessType == NetworkAccess.Internet)
                {
                    var (geoloc, weather) = await dataService.GetData(requestedName);

                    Forecasts.Add(ConstructWeatherData(geoloc, weather));
                }

                else
                {
                    await Shell.Current.DisplayAlert("No Internet Connection", "Please check your internet connection and try again.", "OK");
                }

                searchPopup!.Close();
                searchPopup = null;
            }
            catch (ServiceException ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
            // Handling unknown exceptions separately so as to not leak any sensitive information
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", "Unknown error. Please try again later.", "OK");
            }
            finally
            {
                IsWorking = false;
            }
        }

        [RelayCommand]
        void RemoveSwiped(TimeSeriesData forecast)
        {
            Forecasts.Remove(forecast);
        }
    }
}
