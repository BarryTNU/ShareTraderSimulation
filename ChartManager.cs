using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Graphics;
using Syncfusion.Maui.Charts;
using ShareTrader.Services;
using System.Globalization;
using Microsoft.Maui.Controls;
using System.Collections.Generic;

namespace ShareTrader
{
    public static class ChartManager
    {
        public static List<decimal> lst_Closing = new();
        public static List<decimal> lst_High = new();
        public static List<decimal> lst_Low = new();
        public static List<decimal> lst_Volume = new();
        public static List<string> lst_PriceByDate = new();
        public static string FmDate;
        public static string Reccomendations = "";

        public static decimal closePrice;
        public static decimal  sma7 =0m;
        public static decimal sma20 =0m;
        public static decimal sma30 =0m;
        public static decimal currentMACD =0m;
        public static decimal currentSignal =0m;
        public static decimal adx =0m;
        public static decimal rsi =0m;
        public static decimal volume =0m;
        public static decimal avgVolume =0m;
        public static decimal BuyPrice =0m;
        public class ChartPoint
        { public int Index { get; set; }
            public decimal Value { get; set; }
        }

        public static void PlotBollingerBands(SfCartesianChart chart, int period, string CompanyName)
        {
            if (period <= 0)
                period = 30;
            

          //  decimal multiplier = 2.5m;
            // Clear previous chart
            chart.Series.Clear();


            // Make sure company data is loaded
            LoadCompanyData(CompanyName, 100);

            List<decimal> values = lst_Closing;

            List<decimal> upperBand = new ();
            List<decimal> middleBand = new ();
            List<decimal> lowerBand = new ();

            // Calculate Bollinger Bands
            decimal k = 3.0m;

            for (int i = period - 1; i < values.Count; i++)
            {
                List<decimal> window = values
                    .Skip(i - period + 1)
                    .Take(period)
                    .ToList();

                decimal sma = window.Average();

                decimal variance = window
                    .Average(x => (x - sma) * (x - sma));

                decimal std =
                    (decimal)Math.Sqrt((double)variance);

                middleBand.Add(sma);
                upperBand.Add(sma + (k * std));
                lowerBand.Add(sma - (k * std));
            }

            // Create Syncfusion data collections
            List<ChartPoint> middleData = new();
            List<ChartPoint> upperData = new();
            List<ChartPoint> lowerData = new();

            for (int i = 0; i < middleBand.Count; i++)
            {
                int index = i + period - 1;

                middleData.Add(new ChartPoint
                {
                    Index = index,
                    Value = middleBand[i]
                });

                upperData.Add(new ChartPoint
                {
                    Index = index,
                    Value = upperBand[i]
                });

                lowerData.Add(new ChartPoint
                {
                    Index = index,
                    Value = lowerBand[i]
                });
            }

            // Remove previous Bollinger series
            chart.Series.Clear();

            // Middle Band
            LineSeries middleSeries = new ()
            {
                ItemsSource = middleData,
                XBindingPath = "Index",
                YBindingPath = "Value",
                Label = "Bollinger Middle",
                Fill = Colors.Black,
                StrokeWidth = 2
            };

            // Upper Band
            LineSeries upperSeries = new ()
            {
                ItemsSource = upperData,
                XBindingPath = "Index",
                YBindingPath = "Value",
                Label = "Bollinger Upper",
                Fill = Colors.Green,
                StrokeWidth = 2
            };

            // Lower Band
            LineSeries lowerSeries = new ()
            {
                ItemsSource = lowerData,
                XBindingPath = "Index",
                YBindingPath = "Value",
                Label = "Bollinger Lower",
                Fill = Colors.Red,
                StrokeWidth = 2
            };

            chart.Series.Add(middleSeries);
            chart.Series.Add(upperSeries);
            chart.Series.Add(lowerSeries);
        }
        public static void PlotADX(SfCartesianChart chart, string CompanyName)
        {
            // Clear previous chart
            chart.Series.Clear();

            // Make sure company data is loaded
            if (lst_Closing.Count == 0)
            {
                LoadCompanyData(CompanyName, 100);
            }

            List<decimal> plots =
                TechnicalIndicators.ADX(
                    lst_High,
                    lst_Low,
                    lst_Closing,
                    30);

            List<ChartPoint> data = new();

            for (int i = 0; i < plots.Count; i++)
            {
                data.Add(new ChartPoint
                {
                    Index = i,
                    Value = plots[i]
                });
            }

            LineSeries series = new ()
            {
                ItemsSource = data,
                XBindingPath = "Index",
                YBindingPath = "Value",
                Label = "ADX",
                Fill = Colors.Blue,
                StrokeWidth = 2
            };

            chart.Series.Clear();
            chart.Series.Add(series);
        }
        public static void PlotMAS(SfCartesianChart chart, string CompanyName)
        {
            chart.Series.Clear();

            // Make sure company data is loaded
            if (lst_Closing.Count == 0)
            {
                LoadCompanyData(CompanyName, 100);
            }

            // Calculate the 30-day moving average
          //  List<decimal>a30 =
          //      TechnicalIndicators.SMA(lst_Closing, 30);

            List<decimal> maSlope =
                TechnicalIndicators.CalculateMASlope(lst_Closing);

            List<ChartPoint> data = new();
            List<Brush> brushes = new();

            for (int i = 0; i < maSlope.Count; i++)
            {
                data.Add(new ChartPoint
                {
                    Index = i,
                    Value = maSlope[i]
                });

                // Green for positive, red for negative, grey for zero
                if (maSlope[i] > 0)
                    brushes.Add(new SolidColorBrush(Colors.Green));
                else if (maSlope[i] < 0)
                    brushes.Add(new SolidColorBrush(Colors.Red));
                else
                    brushes.Add(new SolidColorBrush(Colors.Gray));
            }

            ColumnSeries series = new ()
            {
                ItemsSource = data,
                XBindingPath = "Index",
                YBindingPath = "Value",
                Label = "MAS",
                Width = 0.8,
                PaletteBrushes = brushes
            };

            chart.Series.Add(series);
        }
        public static List<decimal> LoadCompanyData(string Company, int Startindex)
        {
            if (string.IsNullOrEmpty(Company))
                return new List<decimal>();

            decimal Open = 0m;
            decimal High = 0m;
            decimal Low = 0m;
            decimal Close = 0m;
            decimal Volume = 0m;
            decimal periodHigh = 0m;
            decimal periodLow = decimal.MaxValue;
         //   decimal periodVolume = 0;
            DateTime highdate = DateTime.MinValue; // Initialize to avoid CS0165
            DateTime lowdate = DateTime.MaxValue;  // Initialize to avoid CS0165

            string fPath = "";

            lst_Closing.Clear();
            lst_High.Clear();
            lst_Low.Clear();
            lst_Volume.Clear();
            lst_PriceByDate.Clear();

            // Ensure Startindex is set to 30 if 0
            if (Startindex == 0)
                Startindex = 30;

            if (Company.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                fPath = Path.Combine(AppGlobals.DataPath, Company);
            }
            else
            {
                fPath = Path.Combine(AppGlobals.DataPath, Company + ".csv");
            }

            // Check if file exists
            bool fileExists = File.Exists(fPath);

            High = 0;
            Low = decimal.MaxValue;

            try
            {
                if (fileExists)
                {
                    var lastX = File.ReadLines(fPath)
                        .Reverse()
                        .Take(Startindex)
                        .Reverse()
                        .ToList();

                    foreach (var dLine in lastX)
                    {
                        if (dLine.StartsWith("Date") || string.IsNullOrWhiteSpace(dLine))
                            continue;

                        var parts = dLine.Split(',');
                        if (parts.Length < 6)
                            continue;

                        string dDate = parts[0];
                        Open = decimal.Parse(parts[1], CultureInfo.InvariantCulture);
                        High = decimal.Parse(parts[2], CultureInfo.InvariantCulture);
                        Low = decimal.Parse(parts[3], CultureInfo.InvariantCulture);
                        Close = decimal.Parse(parts[4], CultureInfo.InvariantCulture);
                        Volume = decimal.Parse(parts[5], CultureInfo.InvariantCulture);

                        var Dt = dDate.Split('-');
                        if (Dt.Length >= 3)
                        {
                            FmDate = Dt[2] + "-" + Dt[1] + "-" + Dt[0];
                            // Add to lists
                        }
                        if (periodHigh < High)
                        {
                            periodHigh = High;
                            highdate = DateTime.ParseExact(dDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                        }
                        if (Low < periodLow)
                        {
                            periodLow = Low;
                            lowdate = DateTime.ParseExact(dDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                        }
                        lst_Closing.Add(Close);
                        lst_High.Add(High);
                        lst_Low.Add(Low);
                        lst_Volume.Add(Volume);
                        string PxD = FmDate + ",  Close  " + Close;
                        lst_PriceByDate.Add(PxD);
                    }
                }
            }
            catch (Exception ex)
            {
                //     MessageBox.Show(e.Message);
            }

            AppGlobals.PeriodHigh = periodHigh;
            AppGlobals.Highdate = highdate;
            AppGlobals.PeriodLow = periodLow;
            AppGlobals.Lowdate = lowdate;

            if (lst_Closing != null && lst_Closing.Count > 0)
                return lst_Closing;
            else
                return new List<decimal>();

        }
        //==== Add Prices to PriceByDate box ==========================

        public static List<string> FillPxDateList(string CompanyName)
        {
            // Make sure company data is loaded
            if (lst_PriceByDate.Count == 0)
            {
                LoadCompanyData(CompanyName, 100);
            }

            var lastItems = lst_PriceByDate
            .Skip(Math.Max(0, lst_PriceByDate.Count - 30))
            .Reverse();

            var formattedItems = lastItems.Select(static line =>
            {
                string[] parts = line.Split(' ');

                // Fix for CS1501: Use TryParse with out parameter and handle parse failure
                decimal price;
                decimal.TryParse(parts[^1], out price); // Fix for IDE0056: Use index from end operator

                return $"{string.Join(" ", parts.Take(parts.Length - 1))} {price:C2}";
            }).ToList();

            return formattedItems;
        }

        public static void GetHighLowDates()
        {
          //  decimal High = 0m;
         //   decimal Low = 0m;
          //  decimal temp = 0m;

            foreach (var item in lst_PriceByDate)
             {
               // temp = item.Price
            }   
           
        }

        public static bool ConservativeSellSignal()
       //decimal closePrice,
       //   decimal sma7,
      //    decimal sma20,
      //    decimal sma30,
      //    decimal currentMACD,
      //    decimal currentSignal,
      //    decimal rsi,
      //    decimal buyPrice)
        {
            //==============================================
            // 1. Capital preservation (highest priority)
            //==============================================
            bool stopLoss =
                closePrice <= BuyPrice * 0.94m;      // 6% stop loss

            //==============================================
            // 2. Confirmed trend reversal
            //==============================================
            bool trendReversal =
                sma7 < sma20 &&
                sma20 < sma30 &&
                currentMACD < currentSignal;

            //==============================================
            // 3. Overbought momentum rolling over
            //==============================================
            bool overboughtReversal =
                rsi > 75m &&
                currentMACD < currentSignal;

            //==============================================
            // 4. Price has broken the long-term trend
            //==============================================
            bool priceBelowTrend =
                closePrice < sma30 &&
                currentMACD < currentSignal;

            //==============================================
            // Sell if ANY major warning occurs
            //==============================================
            return stopLoss ||
                   trendReversal ||
                   overboughtReversal ||
                   priceBelowTrend;
        }

        public static bool ConservativeBuySignal()
     //   decimal closePrice,
     //   decimal sma7,
     //   decimal sma20,
     //   decimal sma30,
     //   decimal currentMACD,
     //   decimal currentSignal,
     //   decimal adx,
     //   decimal RS_I,
     //   decimal volume,
     //   decimal avgVolume)
        {
            // 1. Long-term uptrend
            bool trendUp =
                closePrice > sma30 &&
                sma7 > sma20 &&
                sma20 > sma30;
            // 2. Momentum confirmation
            bool momentumPositive =
               currentMACD > currentSignal;
            // 3. Trend strength
            bool strongTrend =
                adx >= 20m;
            // 4. Avoid overbought entries
            bool notOverbought =
                rsi < 70m;
            // 5. Volume confirmation
            bool goodVolume =
                volume > avgVolume;
            return trendUp &&
                   momentumPositive &&
                   strongTrend &&
                   notOverbought &&
                   goodVolume;
        }
      

        public static List<Single> ConvertListToSingle(List<decimal> decimalList)
        {
            var singleList = new List<Single>(decimalList.Count);
            foreach (var d in decimalList)
            {
                singleList.Add((Single)d);
            }
            return singleList;
        }

        public static string BuyOrSell(string CompanyName)
        {
          
            LoadCompanyData(CompanyName, 100);           

            List<float> lstClose = ConvertListToSingle(lst_Closing);

            List<decimal> prices = lst_Closing;
            int Period = 30;
            decimal slope = (decimal)TechnicalIndicators.LinearRegressionSlope(lstClose, Period);
            decimal slopeNorm = slope / prices.Last();

            decimal sma = prices
                .Skip(prices.Count - Period)
                .Take(Period)
                .Average();

            decimal price = prices.Last();

            int volPeriod = Period;

            List<decimal> recent = prices
                .Skip(prices.Count - volPeriod)
                .Take(volPeriod)
                .ToList();

            decimal volatility = (recent.Max() - recent.Min()) / price;

            decimal score = slopeNorm * 100m;   // scale it

            if (price > sma)
                score += 10m;
            else
                score -= 10m;

            if (volatility < 0.01m)
                score += 5m;        // calm → good
            else if (volatility > 0.03m)
                score -= 5m;        // too volatile → risky

            int strength;
           string signal = "";

            if (score > 20m)
                strength = 3;
            else if (score > 10m)
                strength = 2;
            else if (score > 2m)
                strength = 1;
            else if (score < -20m)
                strength = -3;
            else if (score < -10m)
                strength = -2;
            else if (score < -2m)
                strength = -1;
            else
                strength = 0;

            switch (strength)
            {

                case 3:
                    signal = "▲ ▲ ▲";
                    break;

                case 2:
                    signal = "▲ ▲";
                    break;

                case 1:
                    signal = "▲";
                    break;

                case 0:
                    signal = "▶";
                    break;

                case -1:
                    signal = "▼";
                    break;

                case -2:
                    signal = "▼ ▼";
                    break;

                case -3:
                    signal = "▼ ▼ ▼";
                    break;
            }

            return signal;
        }
        //=================For future use maybe=========================

        public static int CalculateTrendScore(
            decimal closePrice,
            decimal sma7,
            decimal sma20,
            decimal sma30,
            decimal macd,
            decimal signal,
            decimal rsi,
            decimal adx,
            decimal volume,
            decimal avgVolume,
            decimal mas)
        {
            int score = 0;

            //-------------------------------------------------------
            // Long-term trend
            //-------------------------------------------------------
            if (closePrice > sma30)
                score += 2;
            else
                score -= 2;

            //-------------------------------------------------------
            // Moving Average alignment
            //-------------------------------------------------------
            if (sma7 > sma20)
                score++;
            else
                score--;

            if (sma20 > sma30)
                score++;
            else
                score--;

            //-------------------------------------------------------
            // MACD
            //-------------------------------------------------------
            if (macd > signal)
                score += 2;
            else
                score -= 2;

            //-------------------------------------------------------
            // RSI
            //-------------------------------------------------------
            if (rsi >= 40m && rsi <= 65m)
                score += 2;
            else if (rsi > 75m)
                score -= 2;
            else if (rsi < 30m)
                score -= 1;

            //-------------------------------------------------------
            // ADX
            //-------------------------------------------------------
            if (adx >= 30m)
                score += 2;
            else if (adx >= 20m)
                score += 1;

            //-------------------------------------------------------
            // Volume
            //-------------------------------------------------------
            if (volume > avgVolume)
                score++;

            //-------------------------------------------------------
            // Moving Average Slope
            //-------------------------------------------------------
            if (mas > 0)
                score += 2;
            else
                score -= 2;

            return score;
        }

        public static string TradingRecommendation(int score)
        {
            if (score >= 9)
                return "Strong Buy";

            if (score >= 6)
                return "Buy";

            if (score >= 3)
                return "Accumulation";

            if (score >= 0)
                return "Hold";

            if (score >= -3)
                return "Reduce";

            if (score >= -6)
                return "Sell";

            return "Strong Sell";
        }

        public static int ConfidenceRating(int score)
        {
            // Score range is approximately -12 to +12
            return Math.Min(100, Math.Abs(score) * 8);
        }


    //   Reccomendations =
  //      @"
//A further refinement

//Because your program already computes a wide range of indicators, I'd split the score into 
//categories rather than having one overall score. For example:

//Trend Score(-5 to +5)
//Momentum Score(-5 to +5)
//Strength Score(-3 to +3)
//Volume Score(-2 to +2)
//Risk Score(-5 to 0)

//Then:

//Overall Score =
//Trend +
//Momentum +
//Strength +
//Volume +
//Risk

//This gives you much more insight.A stock could have:

//Trend      +5
//Momentum   +4
//Strength   +3
//Volume     +2
//Risk       -4
//----------------
//Overall   +10

//Even though the overall score is excellent, the negative Risk Score
//immediately tells you there's something to investigate—perhaps the stock is overbought or unusually volatile.

//Knowing the way your trading application has evolved over the past few weeks,
//I think this multi-component scoring system would become one of its strongest features. 
//It would also make it easy to rank all stocks in your watchlist by overall quality while 
//still showing why each stock received its score. I think it would fit very naturally with the 
//TrendScore, ForecastSignal, and MarketBehaviour concepts you've already started developing.";

    }
}

