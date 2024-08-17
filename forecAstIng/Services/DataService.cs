using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace forecAstIng.Services
{
    public class ServiceException : Exception
    {
        public ServiceException(string message) : base(message) { }
    }

    public class DataService
    {
        HttpClient client;

        public DataService()
        {
            client = new HttpClient();
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
            catch (Exception ex)
            {
                throw new ServiceException("Unable to geolocate.");
            }

            throw new ServiceException($"Bad Server response: {response.StatusCode}. Please check your internet connection and try again.");
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

        private async Task<WeatherData> WeatherFromCoords(double lat, double lon)
        {
            var response = await client.GetAsync($"https://api.open-meteo.com/v1/forecast?latitude={lat}&longitude={lon}&hourly=temperature_2m,relative_humidity_2m,apparent_temperature,precipitation_probability,precipitation,rain,showers,snowfall,weather_code,surface_pressure,visibility,wind_speed_10m,wind_direction_10m,uv_index&daily=weather_code,temperature_2m_max,temperature_2m_min,daylight_duration&timezone=auto&past_days=7");

            if (response.IsSuccessStatusCode)
            {
                WeatherData weather;
                weather = await response.Content.ReadFromJsonAsync<WeatherData>();

                return weather;
            }

            throw new ServiceException($"Bad Server response: {response.StatusCode}. Please check your internet connection and try again.");
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

        public async Task<(Geoloc, WeatherData)> GetData(string requestedName)
        {
            var geocodes = await GeocodeAddress(requestedName);

            // Geocode -> Geoloc for more granular data on the location; Geocodes return specific addresses as display_name,
            // while Geolocs return more detailed information on the location, such as the country, state, city, and more, which
            // we can use to manipulate our TimeSeriesData objects better.

            var geoloc = await GeolocateCoords(double.Parse(geocodes[0].lat), double.Parse(geocodes[0].lon));
            var weather = await WeatherFromCoords(double.Parse(geoloc.lat), double.Parse(geoloc.lon));

            return (geoloc, weather);
        }
    }
}
