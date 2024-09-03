using CommunityToolkit.Maui.Views;
using forecAstIng.Services;
using forecAstIng.View;
using static forecAstIng.ViewModel.TimeFrame;

namespace forecAstIng.ViewModel
{
    public enum TimeFrame { history, today, forecast };

    public partial class ForecastsViewModel : BaseViewModel
    {
        Popup? searchPopup;
        Popup? settingsPopup;
        public ObservableCollection<TimeSeriesData> Forecasts { get; } = new();
        DataService dataService;
        [ObservableProperty]
        bool isRefreshing;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsHistoryOrForecast))]
        TimeFrame timePage;
        // Needed for DataTrigger on MainPage
        public bool IsHistoryOrForecast => TimePage == history || TimePage == forecast;

        public ForecastsViewModel(DataService service)
        {
            Title = "forecAstIng";
            dataService = service;
            TimePage = today;

            LoadLastLocationForecast();
        }

        private WeatherData ConstructWeatherData(Geoloc geoloc, WeatherData weather)
        {
            weather.name = $"{geoloc.address.city}, {geoloc.address.country_code.ToUpperInvariant()}";
            return weather;
        }

        // No granular exception handling; automatic last location forecast adding is a QOL feauture,
        // and the user can attempt to add their location manually if it fails, where they will get more data.
        async Task LoadLastLocationForecast()
        {
            if (IsWorking) return;

            try
            {
                IsWorking = true;

                var (geoloc, weather) = await dataService.GetLastLocationForecast();

                Forecasts.Add(ConstructWeatherData(geoloc, weather));
            }
            catch (Exception)
            {
                await Shell.Current.DisplayAlert("Error", "Attempt to add your location to forecasts automatically failed, consider adding it manually.", "OK");
            }
            finally
            {
                IsWorking = false;
            }
        }

        [RelayCommand]
        async Task GoToMorePage(TimeSeriesData forecast)
        {
            if (forecast is null || IsWorking) return;

            IsWorking = true;

            var toPass = new List<TimeSeriesData> { forecast };

            await Shell.Current.GoToAsync(nameof(MorePage), true, new Dictionary<string, object>
                {
                    {"Forecast", toPass}
                });

            IsWorking = false;
        }

        // A sort of wrapping function for convenience in xaml for main page, also due to how
        // I designed the model (TimeSeriesDataLite has both the time variable and holds its source)
        [RelayCommand]
        async Task GoToHistoryForecastMorePage(TimeSeriesDataLite forecast)
        {
            if (forecast is null || IsWorking) return;

            if (forecast.source is WeatherData weather)
            {
                weather.RefreshBaseDataForDate(forecast.time);
                await GoToMorePage(weather);
                weather.RestoreCurrentDayData();
            }
            else if (forecast.source is StockData stock)
            {
                throw new NotImplementedException();
            }
        }

        // Variable for data triggers on buttons for switching History/Today/Forecast tabs
        [RelayCommand]
        void LoadTimeFrame(TimeFrame timeFrame)
        {
            if (IsWorking) return;

            IsWorking = true;

            TimePage = timeFrame;

            IsWorking = false;
        }

        [RelayCommand]
        void OpenSearchPopup()
        {
            if (IsWorking) return;

            IsWorking = true;

            searchPopup = new SearchPrompt();
            Shell.Current.ShowPopup(searchPopup);

            IsWorking = false;
        }

        [RelayCommand]
        void OpenSettingsPopup()
        {
            if (IsWorking) return;

            IsWorking = true;

            settingsPopup = new SettingsPrompt();
            Shell.Current.ShowPopup(settingsPopup);

            IsWorking = false;
        }

        [RelayCommand]
        async Task AddRequested(string requestedName)
        {
            if (IsWorking) return;

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
            catch (Exception)
            {
                await Shell.Current.DisplayAlert("Error", "Unknown error. Please try again later.", "OK");
            }
            finally
            {
                IsWorking = false;
            }
        }

        [RelayCommand]
        async Task ApplySettings((AppTheme theme, string unit, int interval) parameters)
        {
            bool refreshFlag = false;

            if (DataService.UNIT != parameters.unit)
            {
                DataService.UNIT = parameters.unit;
                refreshFlag = true;
            }

            if (TimeSeriesData.DAYS_OF_HISTORY != parameters.interval)
            {
                TimeSeriesData.DAYS_OF_HISTORY = parameters.interval;
                refreshFlag = true;
            }

            Application.Current.UserAppTheme = parameters.theme;

            if (refreshFlag) await RefreshData();

            settingsPopup!.Close();
            settingsPopup = null;
        }

        [RelayCommand]
        async Task RefreshData()
        {
            if (IsWorking) return;

            // RefreshView implicitly sets IsRefreshing to true, setting IsWorking to true
            // so we don't have to check IsRefreshing separately in our other UI commands.
            IsWorking = true;

            for (var i = 0; i < Forecasts.Count; ++i)
            {
                var dataOld = Forecasts[i];

                if (dataOld is WeatherData weatherOld)
                {
                    var weatherNew = await dataService.WeatherFromCoords(weatherOld.latitude, weatherOld.longitude);
                    weatherNew.name = weatherOld.name;
                    Forecasts[i] = weatherNew;
                }
                else if (dataOld is StockData stockOld)
                {
                    throw new NotImplementedException();
                }
            }

            IsWorking = false;
            IsRefreshing = false;
        }

        [RelayCommand]
        void RemoveSwiped(TimeSeriesData forecast)
        {
            if (IsWorking) return;

            IsWorking = true;

            Forecasts.Remove(forecast);

            IsWorking = false;
        }
    }
}
