namespace forecAstIng.Model
{
    public abstract class TimeSeriesData
    {
        public string Name { get; set; }
        public string MeasurementUnit { get; set; }

        public double HourTimeInterval { get; set; } = 24;
        public double DayTimeInterval => HourTimeInterval / 24;
        public double MinuteTimeInterval => HourTimeInterval * 60;

        public List<double> ValueHistory { get; set; }
        public List<double> ValuePredictions { get; set; }

        public double HistoryHigh { get; set; }
        public double HistoryLow { get; set; }

        public string CurrentBehaviour { get; set; }
        public double CurrentValue { get; set; }

        // Detailed data type attributes, pressure wind... for weather, volatility, liquidty... for stocks
    }

    public class WeatherData : TimeSeriesData
    {
        
    }

    public class StockData : TimeSeriesData
    {
        
    }
    public class Daily
    {
        public List<string> time { get; set; }
        public List<double> temperature_2m_max { get; set; }
        public List<double> temperature_2m_min { get; set; }
        public List<double> daylight_duration { get; set; }
    }

    public class DailyUnits
    {
        public string time { get; set; }
        public string temperature_2m_max { get; set; }
        public string temperature_2m_min { get; set; }
        public string daylight_duration { get; set; }
    }

    public class Hourly
    {
        public List<string> time { get; set; }
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

    public class Weather
    {
        public double latitude { get; set; }
        public double longitude { get; set; }
        public double generationtime_ms { get; set; }
        public int utc_offset_seconds { get; set; }
        public string timezone { get; set; }
        public string timezone_abbreviation { get; set; }
        public double elevation { get; set; }
        public HourlyUnits hourly_units { get; set; }
        public Hourly hourly { get; set; }
        public DailyUnits daily_units { get; set; }
        public Daily daily { get; set; }
    }


}
