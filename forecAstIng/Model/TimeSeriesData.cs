﻿// The naming in Model namespace is inconsistent with the rest of the project.
// This was done to make it internally consistent without needing to specify JsonProperty
// for each property in both ServiceGeoData.cs and TimeSeriesData.cs. The APIs used
// use snake_case.

namespace forecAstIng.Model
{
    public class TimeSeriesDataLite(TimeSeriesData dataSource)
    {
        public int history_length = TimeSeriesData.DAYS_OF_HISTORY;
        public TimeSeriesData source { get; set; } = dataSource;
        public DateTime time { get; set; }
        public string measurement_unit { get; set; }
        public string current_behaviour { get; set; }
        public double daily_value_max { get; set; }
        public double daily_value_min { get; set; }
        public double hour_value { get; set; }
    }

    public class DailyHistory(TimeSeriesData dataSource) : TimeSeriesDataLite(dataSource), IEnumerable<DailyHistory>
    {
        public IEnumerator<DailyHistory> GetEnumerator()
        {
            if (source is WeatherData weatherData)
            {
                for (int i = 0; i < history_length; i++)
                {
                    weatherData.daily.context_current_day = i;

                    var historyDay = new DailyHistory(source)
                    {
                        time = weatherData.daily.time_current,
                        measurement_unit = weatherData.daily_units.temperature_2m_max,
                        current_behaviour = WeatherData.WMOToBehaviour(weatherData.daily.weather_code_current),
                        daily_value_max = weatherData.daily.temperature_2m_max_current,
                        daily_value_min = weatherData.daily.temperature_2m_min_current,
                    };

                    weatherData.daily.context_current_day = history_length;

                    yield return historyDay;
                }
            }

            if (source is StockData stockData)
            {
                throw new NotImplementedException();
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class DailyForecast(TimeSeriesData dataSource) : TimeSeriesDataLite(dataSource), IEnumerable<DailyForecast>
    {
        public IEnumerator<DailyForecast> GetEnumerator()
        {
            if (source is WeatherData weatherData)
            {
                for (int i = 0; i < history_length; i++)
                {
                    weatherData.daily.context_current_day = i + history_length;

                    var historyDay = new DailyForecast(source)
                    {
                        time = weatherData.daily.time_current,
                        measurement_unit = weatherData.daily_units.temperature_2m_max,
                        current_behaviour = WeatherData.WMOToBehaviour(weatherData.daily.weather_code_current),
                        daily_value_max = weatherData.daily.temperature_2m_max_current,
                        daily_value_min = weatherData.daily.temperature_2m_min_current,
                    };

                    weatherData.daily.context_current_day = history_length;

                    yield return historyDay;
                }
            }

            if (source is StockData stockData)
            {
                throw new NotImplementedException();
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class HourlyData(TimeSeriesData dataSource) : TimeSeriesDataLite(dataSource), IEnumerable<HourlyData>
    {
        public IEnumerator<HourlyData> GetEnumerator()
        {
            if (source is WeatherData weatherData)
            {
                for (int i = 0; i < 24; ++i)
                {
                    var temp = weatherData.hourly.local_hours_today;
                    weatherData.hourly.local_hours_today = i;

                    var hour = new HourlyData(source)
                    {
                        time = weatherData.hourly.time_current,
                        measurement_unit = weatherData.hourly_units.temperature_2m,
                        current_behaviour = WeatherData.WMOToBehaviour(weatherData.hourly.weather_code_current),
                        hour_value = weatherData.hourly.temperature_2m_current,
                    };

                    weatherData.hourly.local_hours_today = temp;

                    yield return hour;
                }
            }

            if (source is StockData stockData)
            {
                throw new NotImplementedException();
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public abstract class TimeSeriesData
    {
        public static readonly int DAYS_OF_HISTORY = 7;

        public DailyHistory DailyHistory { get; set; }

        public DailyForecast DailyForecast { get; set; }

        public HourlyData HourlyData { get; set; }

        public TimeSeriesData()
        {
            DailyHistory = new DailyHistory(this);
            DailyForecast = new DailyForecast(this);
            HourlyData = new HourlyData(this);
        }

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
        // Certain setters acting like faux constructors at JSON deserialization.
        // This necessitates an explicit backing field - no autogenerated getter
        // when writing own setter.
        public HourlyUnits hourly_units { 
            get => _hourly_units; 
            set { 
                _hourly_units = value;  
                measurement_unit = value.temperature_2m;
            } }
        private Hourly _hourly;
        public Hourly hourly { 
            get => _hourly; 
            set {
                _hourly = value;
                _hourly.local_hours_today = DateTime.UtcNow.Hour + (int) Math.Floor((double) utc_offset_seconds/3600);
                hour_value = value.temperature_2m_current;
            } }
        public DailyUnits daily_units { get; set; }
        private Daily _daily;
        public Daily daily { 
            get => _daily; 
            set {
                value.daylight_duration = value.daylight_duration.ConvertAll(x => x / 3600);
                _daily = value;
                today_high = value.temperature_2m_max_current;
                today_low = value.temperature_2m_min_current;
                today_behaviour = WMOToBehaviour(value.weather_code_current);
                daily_units.daylight_duration = "h";
            } }

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

        public void RefreshBaseDataForDate(DateTime newDate)
        {
            daily.context_current_day -= daily.time_current.Subtract(newDate).Days;
            hourly.context_current_day = daily.context_current_day;

            hourly_units = _hourly_units;
            hourly = _hourly;
            daily = _daily;
        }

        public void RestoreCurrentDayData() => RefreshBaseDataForDate(DateTime.UtcNow);
    }

    // XAML binding in, for example MorePage, doesn't support indexing through variables in Path,
    // at least I didn't find a better way than making a _current variable for any property I would
    // want to access in this way.. It adds a lot of boilerplate in the model code, but it is more
    // readable in the XAML than indexing like this inside of the View.. It might also help with
    // historical and future data presentation when added, by just changing the current day and
    // current hour variables and calling _current property
    public class Daily
    {
        // Constants for indexing..
        public int context_current_day { get; set; } = TimeSeriesData.DAYS_OF_HISTORY;
        public DateTime time_current => time[context_current_day];
        public List<DateTime> time { get; set; }
        public int weather_code_current => weather_code[context_current_day];
        public List<int> weather_code { get; set; }
        public double temperature_2m_max_current => temperature_2m_max[context_current_day];
        public List<double> temperature_2m_max { get; set; }
        public double temperature_2m_min_current => temperature_2m_min[context_current_day];
        public List<double> temperature_2m_min { get; set; }
        public double daylight_duration_current => daylight_duration[context_current_day];
        public List<double> daylight_duration { get; set; }
        public double uv_index_max_current => uv_index_max[context_current_day];
        public List<double> uv_index_max { get; set; }
        public double precipitation_sum_current => precipitation_sum[context_current_day];
        public List<double> precipitation_sum { get; set; }
        public double precipitation_hours_current => precipitation_hours[context_current_day];
        public List<double> precipitation_hours { get; set; }
        public double precipitation_probability_max_current => precipitation_probability_max[context_current_day];
        public List<int> precipitation_probability_max { get; set; }
    }

    public class DailyUnits
    {
        public string time { get; set; }
        public string weather_code { get; set; }
        public string temperature_2m_max { get; set; }
        public string temperature_2m_min { get; set; }
        public string daylight_duration { get; set; }
        public string uv_index_max { get; set; }
        public string precipitation_sum { get; set; }
        public string precipitation_hours { get; set; }
        public string precipitation_probability_max { get; set; }
    }

    public class Hourly
    {
        // Constants for indexing..
        public int context_current_day { get; set; } = TimeSeriesData.DAYS_OF_HISTORY;
        private int context_current_day_index => context_current_day * 24;
        public int local_hours_today { get; set; }
        public DateTime time_current => time[context_current_day_index + local_hours_today];
        public List<DateTime> time { get; set; } 
        public double temperature_2m_current => temperature_2m[context_current_day_index + local_hours_today];
        public List<double> temperature_2m { get; set; }
        public int relative_humidity_2m_current => relative_humidity_2m[context_current_day_index + local_hours_today];
        public List<int> relative_humidity_2m { get; set; }
        public double apparent_temperature_current => apparent_temperature[context_current_day_index + local_hours_today];
        public List<double> apparent_temperature { get; set; }
        public int precipitation_probability_current => precipitation_probability[context_current_day_index + local_hours_today];
        public List<int> precipitation_probability { get; set; }
        public double precipitation_current => precipitation[context_current_day_index + local_hours_today];
        public List<double> precipitation { get; set; }
        public int weather_code_current => weather_code[context_current_day_index + local_hours_today];
        public List<int> weather_code { get; set; }
        public double visibility_current => visibility[context_current_day_index + local_hours_today];
        public List<double> visibility { get; set; }
        public double wind_speed_10m_current => wind_speed_10m[context_current_day_index + local_hours_today];
        public List<double> wind_speed_10m { get; set; }
        public int wind_direction_10m_current => wind_direction_10m[context_current_day_index + local_hours_today];
        public List<int> wind_direction_10m { get; set; }
    }

    public class HourlyUnits
    {
        public string time { get; set; }
        public string temperature_2m { get; set; }
        public string relative_humidity_2m { get; set; }
        public string apparent_temperature { get; set; }
        public string precipitation_probability { get; set; }
        public string precipitation { get; set; }
        public string weather_code { get; set; }
        public string visibility { get; set; }
        public string wind_speed_10m { get; set; }
        public string wind_direction_10m { get; set; }
    }
}
