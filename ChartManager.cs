using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Graphics;
using Syncfusion.Maui.Charts;
using Microsoft.Maui.Controls;
using ShareTrader.Services;
using System.Globalization;
using Microsoft.Maui.Controls;
using System.Collections.Generic;
using ShareTrader.Helpers;

namespace ShareTrader
{
    public static class ChartManager
    {
        public static List<decimal> lst_Closing = new();
        public static List<decimal> lst_High = new();
        public static List<decimal> lst_Low = new();
        public static List<decimal> lst_Volume = new();
        public static List<string> lst_PriceByDate = new();
        public static string FmDate ="";
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

            InitializeChart(chart);
       
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
            int start = period - 1;

            var middleData = CreateChartPoints(middleBand, start);
            var upperData = CreateChartPoints(upperBand, start);
            var lowerData = CreateChartPoints(lowerBand, start);

            // Add the three Bollinger Band lines.
            chart.Series.Add(CreateLineSeries(middleData, Colors.Black));
            chart.Series.Add(CreateLineSeries(upperData, Colors.Green));
            chart.Series.Add(CreateLineSeries(lowerData, Colors.Red));
        }
      
        public static void PlotADX(SfCartesianChart chart, string CompanyName)
        {

            InitializeChart(chart);
            

            // Make sure company data is loaded
            LoadCompanyData(CompanyName, 100);
           

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

            chart.Series.Add(CreateLineSeries(data, Colors.Blue));

            AddReferenceLines(chart, "ADX");

        }

        public static void PlotLRS(SfCartesianChart chart, string companyName)
        {
            InitializeChart(chart);

            LoadCompanyData(companyName, 100);

            // Calculate 30-day Linear Regression Slope.
            List<decimal> lrs = TechnicalIndicators.LinearRegressionSlope(lst_Closing, 30);

            // Smooth the display with a 5-day EMA.
            lrs = TechnicalIndicators.CalculateEMA(lrs, 5);

            // Scale for easier reading.
            for (int i = 0; i < lrs.Count; i++)
            {
                if (lrs[i] != decimal.MinValue)
                    lrs[i] *= 100m;
            }

            var upTrend = new List<ChartPoint>();
            var weakening = new List<ChartPoint>();
            var downTrend = new List<ChartPoint>();

            for (int i = 0; i < lrs.Count; i++)
            {
                if (lrs[i] == decimal.MinValue)
                    continue;

                decimal current = lrs[i];
                decimal previous = (i == 0 || lrs[i - 1] == decimal.MinValue)
                    ? current
                    : lrs[i - 1];

                // Start with gaps in all three series.
                var green = new ChartPoint { Index = i, Value = decimal.MinValue };
                var blue = new ChartPoint { Index = i, Value = decimal.MinValue };
                var red = new ChartPoint { Index = i, Value = decimal.MinValue };

                if (current >= 0)
                {
                    if (current > previous)
                        green.Value = current;      // Bullish and strengthening.
                    else
                        blue.Value = current;       // Bullish but weakening.
                }
                else
                {
                    if (current < previous)
                        red.Value = current;        // Bearish and strengthening.
                    else
                        blue.Value = current;       // Bearish but recovering.
                }

                upTrend.Add(green);
                weakening.Add(blue);
                downTrend.Add(red);
            }

            // Draw coloured trend segments.
            AddColouredLineRuns(chart, lrs);

            // Grey zero reference line.
            AddReferenceLines(chart, "LRS");
            AddReferenceLines(chart, "LRS");
        }

        private static void AddColouredLineRuns(
    SfCartesianChart chart,
    List<decimal> values)
        {
            if (values.Count < 2)
                return;

            List<ChartPoint> segment = new();
            Color currentColour = Colors.Gray;

            for (int i = 1; i < values.Count; i++)
            {
                if (values[i] == decimal.MinValue || values[i - 1] == decimal.MinValue)
                    continue;

                decimal previous = values[i - 1];
                decimal current = values[i];

                Color colour;

                if (current >= 0)
                    colour = current >= previous ? Colors.ForestGreen : Colors.RoyalBlue;
                else
                    colour = current <= previous ? Colors.Firebrick : Colors.RoyalBlue;

                // Start a new coloured segment.
                if (segment.Count == 0 || colour != currentColour)
                {
                    if (segment.Count > 1)
                        chart.Series.Add(CreateSplineSeries(segment, currentColour, 3));

                    segment = new List<ChartPoint>
            {
                new ChartPoint { Index = i - 1, Value = previous }
            };

                    currentColour = colour;
                }

                segment.Add(new ChartPoint
                {
                    Index = i,
                    Value = current
                });
            }

            // Draw the final segment.
            if (segment.Count > 1)
                chart.Series.Add(CreateLineSeries(segment, currentColour, 3));
        }


        public async static void PlotMAS(SfCartesianChart chart, string CompanyName)
        {
            InitializeChart(chart);

            // Make sure company data is loaded
            LoadCompanyData(CompanyName, 100);
           

            // Calculate the 30-day moving average
          //  List<decimal>a30 =
          //      TechnicalIndicators.SMA(lst_Closing, 30);

            List<decimal> maSlope =
                TechnicalIndicators.CalculateMASlope(lst_Closing);

            List<ChartPoint> data = new();
            List<Brush> brushes = new();


         //   var data = CreateChartPoints(maSlope);

            for (int i = 0; i < maSlope.Count; i++)
            {
                data.Add(new ChartPoint
                {
                    Index = i,
                    Value = maSlope[i]
                });

                // Forest Green for positive, Firebrick for negative, Light Gray for zero.
                if (maSlope[i] > 0)
                    brushes.Add(new SolidColorBrush(Colors.ForestGreen));
                else if (maSlope[i] < 0)
                    brushes.Add(new SolidColorBrush(Colors.Firebrick));
                else
                    brushes.Add(new SolidColorBrush(Colors.LightGray));
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

    

        public static void PlotVolume(SfCartesianChart chart, string CompanyName)
        {
            int days = 0;

            InitializeChart(chart);
           

            // Make sure company data is loaded
            LoadCompanyData(CompanyName, 100);
          
            try
            {

                if (days <= lst_Volume.Count)
                {
                    days = lst_Volume.Count;
                }

              var volumeData = CreateChartPoints(lst_Volume);

              var volumeSeries = new ColumnSeries
                {
                    Label = "Volume",
                    ItemsSource = volumeData,
                    XBindingPath = "Index",
                    YBindingPath = "Value"
                };

                chart.Series.Add(volumeSeries);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"Error Plotting Volume: {ex.Message}");
            }
        }



        public static void PlotATR(SfCartesianChart chart, string CompanyName)
        {
            int days = 0;

            InitializeChart(chart);
                      

            // Make sure company data is loaded
            LoadCompanyData(CompanyName, 100);

          
            // Calculate ATR.
            var result = TechnicalIndicators.ATR(
                lst_High,
                lst_Low,
                lst_Closing,
                14);

            if (days <= result.Count)
            {
                days = result.Count;
            }

            // Create chart data points.
            var data = CreateChartPoints(result);


            // Create the ATR line.
            chart.Series.Add(CreateLineSeries(data, Colors.DarkOrange));
        }

      
        public static void PlotRSI(SfCartesianChart chart, string CompanyName)
        {
            int days = 0;

            InitializeChart(chart);
                      

            // Make sure company data is loaded.
            LoadCompanyData(CompanyName, 100);

            try
            {
              
                // Calculate RSI.
                var result = TechnicalIndicators.RSI(
                    lst_Closing,
                    14);

                if (days <= result.Count)
                {
                    days = result.Count;
                }

                // Create chart data points.
                 var data = CreateChartPoints(result);

                // Create the RSI line.
                chart.Series.Add(CreateLineSeries(data, Colors.Red));

                AddReferenceLines(chart, "RSI");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"Error Plotting RSI: {ex.Message}");
            }
        }

  

        public static void PlotOBV(SfCartesianChart chart, string CompanyName)
        {

            InitializeChart(chart);

           
            // Make sure company data is loaded.
            LoadCompanyData(CompanyName, 100);

            // Calculate OBV values
            List<decimal> result = TechnicalIndicators.OBV(lst_Closing, lst_Volume);

            // Create chart data
         var data = CreateChartPoints(result);

            // Create the OBV line series
            chart.Series.Add(CreateLineSeries(data, Colors.DeepSkyBlue));

        }

        public static void PlotWilliams(SfCartesianChart chart, string companyName)
        {

            InitializeChart(chart);          

            // Make sure company data is loaded.
            LoadCompanyData(companyName, 100);

            try
            {
                 // Calculate Williams %R.
                List<decimal> williamsValues = TechnicalIndicators.WilliamsR(
                    lst_High,
                    lst_Low,
                    lst_Closing,
                    14);

                // Create chart data, skipping the warm-up values.
                  var williamsData = CreateChartPoints(williamsValues, 1, true);

                // Create Williams %R line series.
                chart.Series.Add(CreateLineSeries(williamsData, Colors.BlueViolet));

                // Set axis titles and Williams %R range.
                if (chart.YAxes.Count > 0)
                {
                    if (chart.YAxes[0] is NumericalAxis yAxis)
                    {
                        yAxis.Minimum = -100;
                        yAxis.Maximum = 0;
                        yAxis.Interval = 20;                       
                    }
                }

                AddReferenceLines(chart, "WILL");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"Error Plotting Williams: {ex.Message}");
            }
        }


        public static void PlotStochastic(SfCartesianChart chart, string companyName)
        {
            InitializeChart(chart);
          

            // Make sure company data is loaded.
            LoadCompanyData(companyName, 100);

            try
            {
                // Calculate Stochastic Oscillator (%K).
                List<decimal> stochasticValues = TechnicalIndicators.Stochastic(
                    lst_High,
                    lst_Low,
                    lst_Closing,
                    14);

                // Create chart data, skipping warm-up values.
               var stochasticData = CreateChartPoints(stochasticValues, 1, true);

                // Create the Stochastic line series.
                chart.Series.Add(CreateLineSeries(stochasticData, Colors.SeaGreen));

                // Set Y-axis range to 0–100.
                if (chart.YAxes.Count > 0 && chart.YAxes[0] is NumericalAxis yAxis)
                {
                    yAxis.Minimum = 0;
                    yAxis.Maximum = 100;
                    yAxis.Interval = 20;                   
                }
                              

                AddReferenceLines(chart, "STOC");

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"Error plotting Stochastic: {ex.Message}");
            }
        }

      

        public static void PlotMACD(SfCartesianChart chart, string companyName)
        {

            InitializeChart(chart);

           
            // Load last 100 days of company data.
            LoadCompanyData(companyName, 100);

            try
            {
                 chart.Annotations.Clear();

                // Calculate MACD, Signal and Histogram.
                MACDResult macd = TechnicalIndicators.CalculateMACD(lst_Closing);                         

                var macdData = CreateChartPoints(macd.MACD, 1, true);
                var signalData = CreateChartPoints(macd.Signal, 1, true);
                var histogramData = CreateChartPoints(macd.Histogram, 1, true);


                chart.PaletteBrushes = new List<Brush>
                {
                    new SolidColorBrush(Colors.ForestGreen), // Histogram
                    new SolidColorBrush(Colors.DodgerBlue),  // MACD
                    new SolidColorBrush(Colors.Orange)       // Signal
                };


                // ---------- Histogram ----------
                chart.Series.Add(new ColumnSeries
                {
                    ItemsSource = histogramData,
                    XBindingPath = nameof(ChartPoint.Index),
                    YBindingPath = nameof(ChartPoint.Value),
                    EnableTooltip = false
                });

                // ---------- MACD Line ----------
                chart.Series.Add(new LineSeries
                {
                    ItemsSource = macdData,
                    XBindingPath = nameof(ChartPoint.Index),
                    YBindingPath = nameof(ChartPoint.Value),
                    StrokeWidth = 2.5,
                    EnableTooltip = false
                });

                // ---------- Signal Line ----------
                chart.Series.Add(new LineSeries
                {
                    ItemsSource = signalData,
                    XBindingPath = nameof(ChartPoint.Index),
                    YBindingPath = nameof(ChartPoint.Value),
                    StrokeWidth = 2,
                    EnableTooltip = false
                });

                // ---------- Zero Line ----------
                chart.Annotations.Add(new HorizontalLineAnnotation
                {
                    Y1 = 0,
                    Stroke = Colors.Gray,
                    StrokeWidth = 1
                });

                // ---------- Tidy up chart appearance ----------

                // Hide the legend (or remove this line if you never show a legend)
                chart.Legend = null;

                // ---------- Axes ----------
                chart.XAxes.Clear();
                chart.YAxes.Clear();

                chart.XAxes.Add(new NumericalAxis
                {
                    IsVisible = true
                });

                chart.YAxes.Add(new NumericalAxis
                {
                    IsVisible = true
                });

                AddReferenceLines(chart, "MACD");
            }
           

            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"Error plotting MACD: {ex.Message}");
            }
        }

        public static void PlotROC(SfCartesianChart chart, string companyName)
        {
            InitializeChart(chart);          

            const int Days = 100;
            const int RocPeriod = 14;

            // Load the last 100 days of company data.
            LoadCompanyData(companyName, Days);

            // Calculate the 14-day Rate of Change.
            List<decimal> rocValues = TechnicalIndicators.ROC(lst_Closing, RocPeriod);

            // Create the chart data source.
            var chartData = CreateChartPoints(rocValues);


            // ROC line.
            chart.Series.Add(CreateLineSeries(chartData, Colors.Brown));

            AddReferenceLines(chart, "ROC");
        }


        public static void AddReferenceLines(SfCartesianChart chart, string indicatorCode)
        {
            // Remove only existing reference line annotations.
            chart.Annotations.Clear();

            switch (indicatorCode.ToUpper())
            {
                case "RSI":
                    AddReferenceLine(chart, 70, Colors.Red);
                    AddReferenceLine(chart, 50, Colors.Gray);
                    AddReferenceLine(chart, 30, Colors.Green);
                    break;

                case "STOC":
                    AddReferenceLine(chart, 80, Colors.Red);
                    AddReferenceLine(chart, 20, Colors.Green);
                    break;

                case "WILL":
                    AddReferenceLine(chart, -20, Colors.Red);
                    AddReferenceLine(chart, -80, Colors.Green);
                    break;

                case "MACD":
                case "ROC":
                    AddReferenceLine(chart, 0, Colors.Gray);
                    break;

                case "ADX":
                    AddReferenceLine(chart, 25, Colors.Orange);
                    break;

                case "LRS":
                    AddReferenceLine(chart, 0, Colors.Gray);
                    break; 

            }
        }

        private static void AddReferenceLine(SfCartesianChart chart, double y, Color color)
        {
            chart.Annotations.Add(new HorizontalLineAnnotation
            {
                Y1 = y,
                Stroke = color,
                StrokeWidth = 1,
                StrokeDashArray = new DoubleCollection { 4, 4 }
            });
        }

        //===================================================
        //          END OF PLOTTING ROUTINES
        //====================================================

        public static List<decimal> LoadCompanyData(string Company, int startindex)
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

            // Ensure startindex is set to 30 if 0
            if (startindex == 0)
                startindex = 30;

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
                        .Take(startindex)
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
                CustomMessageBox.DefaultFocus = DefaultButton.OK;
                CustomMessageBox.ShowAsync!(
                "Loading Data Error",
                $"{ex.Message}",        
                MessageType.Warning);                       
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
      

        public static string BuyOrSell(string companyName)
        {
            LoadCompanyData(companyName, 100);

            const int period = 30;

            List<decimal> prices = lst_Closing;

            decimal price = prices.Last();

            // Last 30 prices.
            List<decimal> recent = prices.TakeLast(period).ToList();

            // Current Linear Regression Slope (last calculated value).
            decimal slope = TechnicalIndicators.CurrentLinearRegressionSlope(prices, period);
            decimal slopeNorm = slope / price;

            // 30-day SMA.
            decimal sma = recent.Average();

            // Volatility.
            decimal volatility = (recent.Max() - recent.Min()) / price;

            // Score.
            decimal score = slopeNorm * 100m;

            score += price > sma ? 10m : -10m;

            if (volatility < 0.01m)
                score += 5m;
            else if (volatility > 0.03m)
                score -= 5m;

            // Determine strength.
            int strength = score switch
            {
                > 20m => 3,
                > 10m => 2,
                > 2m => 1,
                < -20m => -3,
                < -10m => -2,
                < -2m => -1,
                _ => 0
            };

            // Convert strength to signal.
            return strength switch
            {
                3 => "▲ ▲ ▲",
                2 => "▲ ▲",
                1 => "▲",
                0 => "▶",
                -1 => "▼",
                -2 => "▼ ▼",
                -3 => "▼ ▼ ▼",
                _ => "▶"
            };
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

        private static void InitializeChart(SfCartesianChart chart)
        {
            chart.Series.Clear();
            chart.Annotations.Clear();
            chart.XAxes.Clear();
            chart.YAxes.Clear();
            chart.Legend = null;

            chart.XAxes.Add(new NumericalAxis
            {
                IsVisible = true,

                LabelStyle = new ChartAxisLabelStyle
                {
                    FontSize = 9        // Default is about 12–14
                },

                MajorTickStyle = new ChartAxisTickStyle
                {
                    TickSize = 3         // Shorter tick marks
                }
            });

            chart.YAxes.Add(new NumericalAxis
            {
                IsVisible = true,
                Minimum = double.NaN,
                Maximum = double.NaN,

                LabelStyle = new ChartAxisLabelStyle
                {
                    FontSize = 9
                },

                MajorTickStyle = new ChartAxisTickStyle
                {
                    TickSize = 3
                }
            });
        }

        private static List<ChartPoint> CreateChartPoints(
    List<decimal> values,
    int startIndex = 0,
    bool skipInvalid = false)
        {
            var points = new List<ChartPoint>();

            for (int i = 0; i < values.Count; i++)
            {
                if (skipInvalid && values[i] == decimal.MinValue)
                    continue;

                points.Add(new ChartPoint
                {
                    Index = i + startIndex,
                    Value = values[i]
                });
            }

            return points;
        }

        private static LineSeries CreateLineSeries(
       List<ChartPoint> points,
       Color colour,
       double width = 2)
        {
            return new LineSeries
            {
                ItemsSource = points,
                XBindingPath = nameof(ChartPoint.Index),
                YBindingPath = nameof(ChartPoint.Value),
                Fill = colour,
                StrokeWidth = width,
                EnableTooltip = false,
            };
        }

        private static SplineSeries CreateSplineSeries(
    List<ChartPoint> points,
    Color colour,
    double width = 3)
        {
            return new SplineSeries
            {
                ItemsSource = points,
                XBindingPath = nameof(ChartPoint.Index),
                YBindingPath = nameof(ChartPoint.Value),
                Fill = colour,
                StrokeWidth = width,
                EnableTooltip = false
            };
        }


    }
}

