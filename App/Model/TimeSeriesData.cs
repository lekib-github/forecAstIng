// The naming in Model namespace is inconsistent with the rest of the project.
// This was done to make it internally consistent without needing to specify JsonPropertyName
// for each property in both ServiceGeoData.cs and TimeSeriesData.cs. The APIs used
// use snake_case.

namespace forecAstIng.Model
{
    public class TimeSeriesDataLite(TimeSeriesData dataSource)
    {
        public int history_length => TimeSeriesData.DAYS_OF_HISTORY;
        public TimeSeriesData source { get; set; } = dataSource;
        public DateTime time { get; set; }
        public string measurement_unit { get; set; }
        public string current_behaviour { get; set; }
        public double daily_value_max { get; set; }
        public double daily_value_min { get; set; }
        public double hour_value { get; set; }
        // Below only used in HourlyData and only on Today and Forecasts MorePage
        public double ml_pred_temperature_2m_hour_value { get; set; }
        public double ml_pred_relative_humidity_hour_value { get; set; }
        public double ml_pred_apparent_temperature_hour_value { get; set; }
        public double ml_pred_precipitation_hour_value { get; set; }
        public double ml_pred_wind_speed_10m_hour_value { get; set; }
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
                var nonTradingDayOffset = 0;
                var reverseOrderDates = new List<DailyHistory>();

                for (int i = 1; i <= history_length; i++)
                {
                    var pastTime = stockData.dailyData.metadata.lastRefreshed.Subtract(TimeSpan.FromDays(i+nonTradingDayOffset));
                    while (!stockData.dailyData.DailyValues.ContainsKey(pastTime))
                    {
                        nonTradingDayOffset++;
                        pastTime = stockData.dailyData.metadata.lastRefreshed.Subtract(TimeSpan.FromDays(i + nonTradingDayOffset));
                    }
                    
                    var pastValue = stockData.dailyData.DailyValues[pastTime];

                    reverseOrderDates.Add(new DailyHistory(source)
                    {
                        time = pastTime,
                        measurement_unit = stockData.fundamentals.Currency,
                        current_behaviour = pastValue.open > pastValue.close ? "down_today" : "up_today",
                        daily_value_max = pastValue.high,
                        daily_value_min = pastValue.low,
                    });
                }

                for (int i = reverseOrderDates.Count - 1; i >= 0; i--) yield return reverseOrderDates[i];
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
                // Don't have forecats data for stocks yet.
                yield return new DailyForecast(source)
                {
                    measurement_unit = "The future is uncertain."
                };

                yield break;
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
                    weatherData.hourly_ml_model_predictions.local_hours_today = i;

                    var hour = new HourlyData(source)
                    {
                        time = weatherData.hourly.time_current,
                        measurement_unit = weatherData.hourly_units.temperature_2m,
                        current_behaviour = WeatherData.WMOToBehaviour(weatherData.hourly.weather_code_current),
                        hour_value = weatherData.hourly.temperature_2m_current,
                    };

                    if (weatherData.hourly_ml_model_predictions.context_current_day >= 0)
                    {
                        hour.ml_pred_temperature_2m_hour_value = weatherData.hourly_ml_model_predictions.temperature_2m_current;
                        hour.ml_pred_relative_humidity_hour_value = weatherData.hourly_ml_model_predictions.relative_humidity_2m_current;
                        hour.ml_pred_apparent_temperature_hour_value = weatherData.hourly_ml_model_predictions.apparent_temperature_current;
                        hour.ml_pred_precipitation_hour_value = weatherData.hourly_ml_model_predictions.precipitation_current;
                        hour.ml_pred_wind_speed_10m_hour_value = weatherData.hourly_ml_model_predictions.wind_speed_10m_current;
                    }

                    weatherData.hourly.local_hours_today = temp;
                    weatherData.hourly_ml_model_predictions.local_hours_today = temp;

                    yield return hour;
                }
            }

            if (source is StockData stockData)
            {
                var contextDay = stockData.intradayData.metadata.lastRefreshed;
                var openTime = stockData.intradayData.IntradayValues.Where(x => x.Key.Date == contextDay.Date).Min(x => x.Key.Hour);
                var closeTime = stockData.intradayData.IntradayValues.Where(x => x.Key.Date == contextDay.Date).Max(x => x.Key.Hour);

                for (int i = openTime; i <= closeTime; i++)
                {
                    var transition = new TimeSpan(i, 0, 0);
                    contextDay = contextDay.Date + transition;

                    var pastValue = stockData.intradayData.IntradayValues[contextDay];

                    yield return new HourlyData(source)
                    {
                        time = contextDay,
                        measurement_unit = stockData.fundamentals.Currency,
                        current_behaviour = pastValue.open > pastValue.close ? "down_today" : "up_today",
                        hour_value = pastValue.close
                    };
                }
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public abstract class TimeSeriesData
    {
        public static int DAYS_OF_HISTORY = 5;

        public DailyHistory DailyHistory { get; set; }

        public DailyForecast DailyForecast { get; set; }

        public HourlyData HourlyData { get; set; }

        public bool HasMLPredictions { get; set; }

        public TimeSeriesData()
        {
            DailyHistory = new DailyHistory(this);
            DailyForecast = new DailyForecast(this);
            HourlyData = new HourlyData(this);
        }

        public string name { get; set; }
        public string measurement_unit { get; set; }

        public double today_high { get; set; }
        public double today_low { get; set; }

        public string today_behaviour { get; set; }
        public double hour_value { get; set; }
    }
}
