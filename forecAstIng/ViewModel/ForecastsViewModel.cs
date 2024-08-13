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

            // TODO: Fix data service method which parses json like a geocode response, need new class for reverse geocode
            //LoadLastLocationForecast();
        }

        async Task LoadLastLocationForecast()
        {
            try
            {
                var forecast = await dataService.GetLastLocation();
                Forecasts.Add(forecast);
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
                    var dataPoint = await dataService.GetData(requestedName);

                    Forecasts.Add(dataPoint);
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
