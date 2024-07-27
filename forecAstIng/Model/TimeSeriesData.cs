using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace forecAstIng.Model
{
    public abstract class TimeSeriesData
    {
        public string Name { get; set; }
        public char MeasurementUnit { get; set; }

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

    class WeatherData : TimeSeriesData
    {

    }

    class StockData : TimeSeriesData
    {

    }
}
