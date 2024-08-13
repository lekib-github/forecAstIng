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

        internal class Geocode
        {
            public int place_id { get; set; }
            public string licence { get; set; }
            public string osm_type { get; set; }
            public object osm_id { get; set; }
            public List<string> boundingbox { get; set; }
            public string lat { get; set; }
            public string lon { get; set; }
            public string display_name { get; set; }
            public string @class { get; set; }
            public string type { get; set; }
            public double importance { get; set; }
        }


        // No granular exception handling; automatic last location forecast adding is a QOL feauture, and the user can attempt
        // to add their location manually if it fails, where they will get more data.
        public async Task<TimeSeriesData> GetLastLocation()
        {
            Location location = await Geolocation.Default.GetLastKnownLocationAsync();

            if (location != null)
            {
                var response = await client.GetAsync($"https://geocode.maps.co/reverse?lat={location.Latitude}&lon={location.Longitude}&api_key={ServiceSecrets.GEOCODEMAPSCO_KEY}");

                if (response.IsSuccessStatusCode)
                {
                    var geocode = await response.Content.ReadFromJsonAsync<List<Geocode>>();
                    return new WeatherData { Name = geocode[0].display_name };
                }
            }  

            throw new ServiceException("No location data found. Please check your location settings and try again.");
        }

        public async Task<TimeSeriesData> GetData(string requestedName)
        {
            List<Geocode> geocodes;

            var response = await client.GetAsync($"https://geocode.maps.co/search?q={requestedName}&api_key={ServiceSecrets.GEOCODEMAPSCO_KEY}");
            if (response.IsSuccessStatusCode)
            {
                geocodes = await response.Content.ReadFromJsonAsync<List<Geocode>>();
                if (geocodes.Count == 0)
                {
                    throw new ServiceException("No match found. Please check the entered location and try again.");
                }

                // Ordering by importance, intuitive; then by length of name, with assumption that shorter names are less specific,
                // and therefore more likely to be the desired location (we take only one off the top for each request)...
                // Eager evaluation shouldn't be a problem, not many geocodes will be returned, in the order of 10s..
                geocodes = geocodes.OrderByDescending(code => code.importance).ThenBy(code => code.display_name.Length).ToList();

                // TODO: establish what data is important for the app to display, and build weather api requests based on that.
                // Then, build a WeatherData object with the data from the response.
                return new WeatherData { Name = geocodes[0].display_name };
            }

            throw new ServiceException($"Bad Server response: {response.StatusCode}. Please check your internet connection and try again.");
        }
    }
}
