using System.Text.Json.Serialization;

namespace forecAstIng.Model
{
    public class SymbolMatches
    {
        [JsonPropertyName("bestMatches")]
        public List<Match> bestMatches { get; set; }
    }

    // Weird choice of names for the properties returned by API, so we unfortunately use JsonPropertyName tag everywhere here.
    public class Match
    {
        [JsonPropertyName("1. symbol")]
        public string symbol { get; set; }

        [JsonPropertyName("2. name")]
        public string name { get; set; }

        [JsonPropertyName("3. type")]
        public string type { get; set; }

        [JsonPropertyName("4. region")]
        public string region { get; set; }

        [JsonPropertyName("5. marketOpen")]
        public string marketOpen { get; set; }

        [JsonPropertyName("6. marketClose")]
        public string marketClose { get; set; }

        [JsonPropertyName("7. timezone")]
        public string timezone { get; set; }

        [JsonPropertyName("8. currency")]
        public string currency { get; set; }

        [JsonPropertyName("9. matchScore")]
        public string matchScore { get; set; }
    }

    public class StockData : TimeSeriesData
    {
        private IntradayData _intradayData;
        public IntradayData intradayData
        {
            get => _intradayData;
            set
            {
                _intradayData = value;
                hour_value = value.IntradayValues[value.metadata.lastRefreshed].close;
            }
        }

        private DailyData _dailyData;
        public DailyData dailyData
        {
            get => _dailyData;
            set
            {
                _dailyData = value;

                var todayEntry = value.DailyValues[value.metadata.lastRefreshed];
                today_high = todayEntry.high;
                today_low = todayEntry.low;
                today_behaviour = todayEntry.open > todayEntry.close ? "down_today" : "up_today";
            } 
        }

        private Fundamentals _fundamentals;
        public Fundamentals fundamentals {
            get => _fundamentals;
            set
            {
                _fundamentals = value;
                name = $"{value.Symbol}, {value.Exchange}";
                measurement_unit = value.Currency;
            }
        }

        public void RefreshBaseDataForDate(DateTime newDate)
        {
            intradayData.metadata._lastRefreshed = newDate.ToShortDateString() + " " + intradayData.metadata._lastRefreshed.Split()[1];
            dailyData.metadata._lastRefreshed = newDate.ToShortDateString();

            intradayData = _intradayData;
            dailyData = _dailyData;
        }
    }

    public class IntradayData
    {
        [JsonPropertyName("Meta Data")]
        public MetaDataIntraday metadata { get; set; }

        // For some reason system.text.json doesn't know how to parse this DateTime so we
        // have to do it like this..
        [JsonPropertyName("Time Series (60min)")]
        [JsonInclude]
        private Dictionary<string, Prices> _IntradayValues 
        {
            get => null;
            set
            {
                foreach (var entry in value)
                {
                    IntradayValues.Add(DateTime.Parse(entry.Key), entry.Value);
                }
            }
        }

        public Dictionary<DateTime, Prices> IntradayValues { get; set; } = [];
    }

    public class DailyData
    {
        [JsonPropertyName("Meta Data")]
        public MetaDataDaily metadata { get; set; }

        [JsonPropertyName("Time Series (Daily)")]
        [JsonInclude]
        private Dictionary<string, Prices> _DailyValues 
        { 
            get => null;
            set
            {
                foreach (var entry in value)
                {
                    DailyValues.Add(DateTime.Parse(entry.Key), entry.Value);
                }
            }
        }

        public Dictionary<DateTime, Prices> DailyValues { get; set; } = [];
    }

    public class MetaDataIntraday
    {
        [JsonPropertyName("1. Information")]
        public string information { get; set; }

        [JsonPropertyName("2. Symbol")]
        public string symbol { get; set; }

        // Similar parsing problem as above..
        [JsonPropertyName("3. Last Refreshed")]
        [JsonInclude]
        internal string _lastRefreshed { get; set; }

        public DateTime lastRefreshed => DateTime.Parse(_lastRefreshed);

        [JsonPropertyName("4. Interval")]
        public string interval { get; set; }

        [JsonPropertyName("5. Output Size")]
        public string outputSize { get; set; }

        [JsonPropertyName("6. Time Zone")]
        public string timezone { get; set; }
    }

    public class MetaDataDaily
    {
        [JsonPropertyName("1. Information")]
        public string information { get; set; }

        [JsonPropertyName("2. Symbol")]
        public string symbol { get; set; }

        [JsonPropertyName("3. Last Refreshed")]
        [JsonInclude]
        internal string _lastRefreshed { get; set; }

        public DateTime lastRefreshed => DateTime.Parse(_lastRefreshed);

        [JsonPropertyName("4. Output Size")]
        public string outputSize { get; set; }

        [JsonPropertyName("5. Time Zone")]
        public string timezone { get; set; }
    }

    public class Prices
    {
        [JsonPropertyName("1. open")]
        public double open { get; set; }

        [JsonPropertyName("2. high")]
        public double high { get; set; }

        [JsonPropertyName("3. low")]
        public double low { get; set; }

        [JsonPropertyName("4. close")]
        public double close { get; set; }

        [JsonPropertyName("5. volume")]
        public double volume { get; set; }
    }

    public class Fundamentals
    {
        public string Symbol { get; set; }
        public string AssetType { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CIK { get; set; }
        public string Exchange { get; set; }
        public string Currency { get; set; }
        public string Country { get; set; }
        public string Sector { get; set; }
        public string Industry { get; set; }
        public string Address { get; set; }
        public string OfficialSite { get; set; }
        public string FiscalYearEnd { get; set; }
        public DateTime LatestQuarter { get; set; }
        public double MarketCapitalization { get; set; }
        public double EBITDA { get; set; }
        public double PERatio { get; set; }
        public double PEGRatio { get; set; }
        public double BookValue { get; set; }
        public double DividendPerShare { get; set; }
        public double DividendYield { get; set; }
        public double EPS { get; set; }
        public double RevenuePerShareTTM { get; set; }
        public double ProfitMargin { get; set; }
        public double OperatingMarginTTM { get; set; }
        public double ReturnOnAssetsTTM { get; set; }
        public double ReturnOnEquityTTM { get; set; }
        public double RevenueTTM { get; set; }
        public double GrossProfitTTM { get; set; }
        public double DilutedEPSTTM { get; set; }
        public double QuarterlyEarningsGrowthYOY { get; set; }
        public double QuarterlyRevenueGrowthYOY { get; set; }
        public double AnalystTargetPrice { get; set; }
        public double AnalystRatingStrongBuy { get; set; }
        public double AnalystRatingBuy { get; set; }
        public double AnalystRatingHold { get; set; }
        public double AnalystRatingSell { get; set; }
        public double AnalystRatingStrongSell { get; set; }
        public double TrailingPE { get; set; }
        public double ForwardPE { get; set; }
        public double PriceToSalesRatioTTM { get; set; }
        public double PriceToBookRatio { get; set; }
        public double EVToRevenue { get; set; }
        public double EVToEBITDA { get; set; }
        public double Beta { get; set; }

        [JsonPropertyName("52WeekHigh")]
        public double _52WeekHigh { get; set; }

        [JsonPropertyName("52WeekLow")]
        public double _52WeekLow { get; set; }

        [JsonPropertyName("50DayMovingAverage")]
        public double _50DayMovingAverage { get; set; }

        [JsonPropertyName("200DayMovingAverage")]
        public double _200DayMovingAverage { get; set; }
        public double SharesOutstanding { get; set; }
        public DateTime DividendDate { get; set; }
        public DateTime ExDividendDate { get; set; }
    }
}
