// The naming in Model namespace is inconsistent with the rest of the project.
// This was done to make it internally consistent without needing to specify JsonProperty
// for each property in both ServiceGeoData.cs and TimeSeriesData.cs. The APIs used
// use snake_case.

namespace forecAstIng.Model
{
    public abstract class TimeSeriesData
    {
        public static readonly int DAYS_OF_HISTORY = 7 - 1; 
        public string name { get; set; }
        public string measurement_unit { get; set; }
        public string timezone { get; set; }

        public double today_high { get; set; }
        public double today_low { get; set; }

        public string today_behaviour { get; set; }
        public double hour_value { get; set; }
    }

    public class StockData : TimeSeriesData
    { 
    }

    public class WeatherData : TimeSeriesData
    {
        public double latitude { get; set; }
        public double longitude { get; set; }
        public double elevation { get; set; }
        public double generationtime_ms { get; set; }
        public int utc_offset_seconds { get; set; }
        public string timezone_abbreviation { get; set; }
        private HourlyUnits _hourly_units;
        public HourlyUnits hourly_units { get => _hourly_units; set { _hourly_units = value;  measurement_unit = value.temperature_2m;} }
        private Hourly _hourly;
        public Hourly hourly { 
            get => _hourly; 
            set {
                _hourly = value;
                int HoursToday = DateTime.UtcNow.Hour + (int) Math.Floor((double) utc_offset_seconds/3600);
                int HoursToToday = DAYS_OF_HISTORY * 24;
                hour_value = value.temperature_2m[HoursToToday + HoursToday];
                today_high = value.temperature_2m[HoursToToday..(HoursToToday + 24)].Max();
                today_low = value.temperature_2m[HoursToToday..(HoursToToday + 24)].Min();
            } }
        public DailyUnits daily_units { get; set; }
        private Daily _daily;
        public Daily daily { get => _daily; set { _daily = value;  today_behaviour = WMOToBehaviour(value.weather_code[DAYS_OF_HISTORY]); } }


        public static string WMOToBehaviour(int code)
        {
            // Code info from https://www.nodc.noaa.gov/archive/arc0021/0002199/1.1/data/0-data/HTML/WMO-CODE/WMO4677.HTM 
            return code switch
            {
                >= 8 and <= 12 or 19  or >= 40 and <= 49 => "cloudy",
                >= 3 and <= 7 => "partly_cloudy",
                13 or 17 or 29 or >= 95 and <= 99 => "lightning",
                // Need snow icon then rewrite..
                >= 14 and <= 16 or >= 20 and <= 28 or >= 50 and <= 94 => "rainy",
                <= 2 => "sunny",
                18 or >= 30 and <= 38 => "windy",
                _ => "N/A"
            };
        }
    }
    public class Daily
    {
        public List<DateTime> time { get; set; }
        public List<int> weather_code { get; set; }
        public List<double> temperature_2m_max { get; set; }
        public List<double> temperature_2m_min { get; set; }
        public List<double> daylight_duration { get; set; }
    }

    public class DailyUnits
    {
        public string time { get; set; }
        public string weather_code { get; set; }
        public string temperature_2m_max { get; set; }
        public string temperature_2m_min { get; set; }
        public string daylight_duration { get; set; }
    }

    public class Hourly
    {
        public List<DateTime> time { get; set; }
        public List<double> temperature_2m { get; set; }
        public List<int> relative_humidity_2m { get; set; }
        public List<double> apparent_temperature { get; set; }
        public List<int> precipitation_probability { get; set; }
        public List<double> precipitation { get; set; }
        public List<double> rain { get; set; }
        public List<double> showers { get; set; }
        public List<double> snowfall { get; set; }
        public List<int> weather_code { get; set; }
        public List<double> surface_pressure { get; set; }
        public List<double> visibility { get; set; }
        public List<double> wind_speed_10m { get; set; }
        public List<int> wind_direction_10m { get; set; }
        public List<double> uv_index { get; set; }
    }

    public class HourlyUnits
    {
        public string time { get; set; }
        public string temperature_2m { get; set; }
        public string relative_humidity_2m { get; set; }
        public string apparent_temperature { get; set; }
        public string precipitation_probability { get; set; }
        public string precipitation { get; set; }
        public string rain { get; set; }
        public string showers { get; set; }
        public string snowfall { get; set; }
        public string weather_code { get; set; }
        public string surface_pressure { get; set; }
        public string visibility { get; set; }
        public string wind_speed_10m { get; set; }
        public string wind_direction_10m { get; set; }
        public string uv_index { get; set; }
    }
}
