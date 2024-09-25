using System.Net.Http.Json;

namespace forecAstIng.Services
{
    public class ServiceException : Exception
    {
        public ServiceException(string message) : base(message) { }
    }

    public class DataService
    {
        public static string UNIT = "celsius";
        HttpClient client;

        public DataService()
        {
            client = new HttpClient();
        }

        private async Task<List<Geocode>> GeocodeAddress(string address)
        {
            var response = await client.GetAsync($"https://geocode.maps.co/search?q={address}&api_key={ServiceSecrets.GEOCODEMAPSCO_KEY}");

            if (response.IsSuccessStatusCode)
            {
                List<Geocode> geocodes;
                geocodes = await response.Content.ReadFromJsonAsync<List<Geocode>>();

                if (geocodes.Count == 0)
                {
                    throw new ServiceException("No match found. Please check the entered location and try again.");
                }

                return geocodes;
            }

            throw new ServiceException($"Bad Server response: {response.StatusCode}. Please check your internet connection and try again.");
        }

        private async Task<Geoloc> GeolocateCoords(double lat, double lon)
        {
            var response = await client.GetAsync($"https://geocode.maps.co/reverse?lat={lat}&lon={lon}&api_key={ServiceSecrets.GEOCODEMAPSCO_KEY}");

            try
            {
                if (response.IsSuccessStatusCode)
                {
                    Geoloc geoloc;
                    geoloc = await response.Content.ReadFromJsonAsync<Geoloc>();

                    return geoloc;
                }
            }
            catch (Exception)
            {
                throw new ServiceException("Unable to geolocate.");
            }

            throw new ServiceException($"Bad Server response: {response.StatusCode}. Please check your internet connection and try again.");
        }

        public async Task<WeatherData> WeatherFromCoords(double lat, double lon)
        {
            var response = await client.GetAsync($"https://api.open-meteo.com/v1/forecast?latitude={lat}&longitude={lon}&hourly=temperature_2m,relative_humidity_2m,apparent_temperature,precipitation_probability,precipitation,weather_code,visibility,wind_speed_10m,wind_direction_10m&daily=weather_code,temperature_2m_max,temperature_2m_min,daylight_duration,uv_index_max,precipitation_sum,precipitation_hours,precipitation_probability_max&temperature_unit={UNIT}&timezone=auto&past_days={TimeSeriesData.DAYS_OF_HISTORY}&forecast_days={TimeSeriesData.DAYS_OF_HISTORY}");
            var date = DateTime.UtcNow;
            var response_ml_model = await client.GetAsync($"http://localhost/forecast?lat={lat}&lon={lon}&date={date.Year}-{date.Month:D2}-{date.Day:D2}");
            
            if (response.IsSuccessStatusCode)
            {
                WeatherData weather;
                weather = await response.Content.ReadFromJsonAsync<WeatherData>();
                Hourly prediction;
                prediction = await response_ml_model.Content.ReadFromJsonAsync<Hourly>();

                return weather;
            }

            throw new ServiceException($"Bad Server response: {response.StatusCode}. Please check your internet connection and try again.");
        }

        public async Task<SymbolMatches> GetSymbolsFromName(string name)
        {
            var response = await client.GetAsync($"https://www.alphavantage.co/query?function=SYMBOL_SEARCH&keywords={name}&apikey={ServiceSecrets.ALPHAVANTAGE_KEY}");

            if (response.IsSuccessStatusCode)
            {
                SymbolMatches matches;
                matches = await response.Content.ReadFromJsonAsync<SymbolMatches>();

                if (matches.bestMatches.Count == 0)
                {
                    throw new ServiceException("No match found. Please check the entered stock name and try again.");
                }

                return matches;
            }

            throw new ServiceException($"Bad Server response: {response.StatusCode}. Please check your internet connection and try again.");
        }

        public async Task<StockData> GetStockFromSymbol(string symbol)
        {
            var intraday_response = await client.GetAsync($"https://www.alphavantage.co/query?function=TIME_SERIES_INTRADAY&symbol={symbol}&interval=60min&apikey={ServiceSecrets.ALPHAVANTAGE_KEY}");
            var daily_response = await client.GetAsync($"https://www.alphavantage.co/query?function=TIME_SERIES_DAILY&symbol={symbol}&apikey={ServiceSecrets.ALPHAVANTAGE_KEY}");
            var fundamentals_response = await client.GetAsync($"https://www.alphavantage.co/query?function=OVERVIEW&symbol={symbol}&apikey={ServiceSecrets.ALPHAVANTAGE_KEY}");

            if (intraday_response.IsSuccessStatusCode && fundamentals_response.IsSuccessStatusCode && daily_response.IsSuccessStatusCode)
            {
                // Technically what we need from DailyData can be calculated from IntradayData,
                // and it would use 1 less request from the pitiful 25 requests/day the free key
                // provides, but the convenience is too much to pass up.
                IntradayData intradayData;
                DailyData dailyData;
                Fundamentals fundamentals;

                intradayData = await intraday_response.Content.ReadFromJsonAsync<IntradayData>();
                dailyData = await daily_response.Content.ReadFromJsonAsync<DailyData>();
                fundamentals = await fundamentals_response.Content.ReadFromJsonAsync<Fundamentals>();


                return new StockData { intradayData = intradayData, dailyData = dailyData, fundamentals = fundamentals};
            }

            var badCode = !intraday_response.IsSuccessStatusCode ? intraday_response.StatusCode : fundamentals_response.StatusCode;
            throw new ServiceException($"Bad Server response: {badCode}. Please check your internet connection and try again.");
        }

        public async Task<(Geoloc, WeatherData)> GetLastLocationForecast()
        {
            Location location = await Geolocation.Default.GetLastKnownLocationAsync();

            if (location != null)
            {
                var geoloc = await GeolocateCoords(location.Latitude, location.Longitude);
                var weather = await WeatherFromCoords(location.Latitude, location.Longitude);

                return (geoloc, weather);
            }

            throw new ServiceException("No location data found. Please check your location settings and try again.");
        }

        public async Task<(Geoloc, WeatherData)> GetWeatherData(string requestedName)
        {
            var geocodes = await GeocodeAddress(requestedName);

            // Crude way to honor 1 req/s from Geocoding API, otherwise consistent TooManyRequests error codes..
            Thread.Sleep(1000);

            // Geocode -> Geoloc for more granular data on the location; Geocodes return specific addresses as display_name,
            // while Geolocs return more detailed information on the location, such as the country, state, city, and more, which
            // we can use to manipulate our TimeSeriesData objects better.
            var geoloc = await GeolocateCoords(double.Parse(geocodes[0].lat), double.Parse(geocodes[0].lon));

            var weather = await WeatherFromCoords(double.Parse(geoloc.lat), double.Parse(geoloc.lon));

            return (geoloc, weather);
        }

        public async Task<StockData> GetStockData(string requestedName)
        {
            var matches = await GetSymbolsFromName(requestedName);

            return await GetStockFromSymbol(matches.bestMatches[0].symbol);
        }
    }
}
