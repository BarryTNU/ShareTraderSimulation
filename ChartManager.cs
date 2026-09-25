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
        public async static void PlotMAS(SfCartesianChart chart, string CompanyName)
        {
            chart.Series.Clear();

            // Make sure company data is loaded
                 LoadCompanyData(CompanyName, 100);
           

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

    

        public static void PlotVolume(SfCartesianChart chart, string CompanyName)
        {
            int days = 0;

            // Make sure company data is loaded
                LoadCompanyData(CompanyName, 100);
          
            try
            {
                chart.Series.Clear();

                if (days <= lst_Volume.Count)
                {
                    days = lst_Volume.Count;
                }

                var volumeData = new List<ChartPoint>();

                for (int i = 0; i < lst_Volume.Count; i++)
                {
                    volumeData.Add(new ChartPoint
                    {
                        Index = i,
                        Value = lst_Volume[i]
                    });
                }

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

            // Make sure company data is loaded
            LoadCompanyData(CompanyName, 100);

            // Clear anything currently displayed in this chart.
            chart.Series.Clear();

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
            var data = new List<ChartPoint>();

            for (int i = 0; i < result.Count; i++)
            {
                data.Add(new ChartPoint
                {
                    Index = i,
                    Value = result[i]
                });
            }

            // Create the ATR line.
            var series = new LineSeries
            {
                Label = "ATR",
                ItemsSource = data,
                XBindingPath = "Index",
                YBindingPath = "Value",
                StrokeWidth = 2
            };

            chart.Series.Add(series);
        }

              

        public static void PlotRSI(SfCartesianChart chart, string CompanyName)
        {
            int days = 0;

            // Make sure company data is loaded.
            LoadCompanyData(CompanyName, 100);

            try
            {
                // Clear anything currently displayed in this chart.
                chart.Series.Clear();

                // Calculate RSI.
                var result = TechnicalIndicators.RSI(
                    lst_Closing,
                    14);

                if (days <= result.Count)
                {
                    days = result.Count;
                }

                // Create chart data points.
                var data = new List<ChartPoint>();

                for (int i = 0; i < result.Count; i++)
                {
                    data.Add(new ChartPoint
                    {
                        Index = i,
                        Value = result[i]
                    });
                }

                // Create the RSI line.
                var series = new LineSeries
                {
                    Label = "RSI",
                    ItemsSource = data,
                    XBindingPath = "Index",
                    YBindingPath = "Value",
                    StrokeWidth = 2
                    // Stroke = Colors.Red
                };

                chart.Series.Add(series);              
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"Error Plotting RSI: {ex.Message}");
            }
        }

  

        public static void PlotOBV(SfCartesianChart chart, string CompanyName)
        {

            // Make sure company data is loaded.
            LoadCompanyData(CompanyName, 100);

            // Calculate OBV values
            List<decimal> result = TechnicalIndicators.OBV(lst_Closing, lst_Volume);

            // Create chart data
            var chartData = new List<ChartPoint>();

            for (int i = 0; i < result.Count; i++)
            {
                chartData.Add(new ChartPoint
                {
                    Index = i + 1,
                    Value = result[i]
                });
            }

            // Clear any previous series
            chart.Series.Clear();

            // Create the OBV line series
            var series = new LineSeries
            {
                ItemsSource = chartData,
                XBindingPath = nameof(ChartPoint.Index),
                YBindingPath = nameof(ChartPoint.Value),
                StrokeWidth = 2
            };

            chart.Series.Add(series);      
       
        }

        public static void PlotWilliams(SfCartesianChart chart, string companyName)
        {
            // Make sure company data is loaded.
            LoadCompanyData(companyName, 100);

            try
            {
                // Clear anything currently displayed.
                chart.Series.Clear();

                // Calculate Williams %R.
                List<decimal> williamsValues = TechnicalIndicators.WilliamsR(
                    lst_High,
                    lst_Low,
                    lst_Closing,
                    14);

                // Create chart data, skipping the warm-up values.
                var williamsData = new List<ChartPoint>();

                for (int i = 0; i < williamsValues.Count; i++)
                {
                    if (williamsValues[i] == decimal.MinValue)
                        continue;

                    williamsData.Add(new ChartPoint
                    {
                        Index = i + 1,
                        Value = williamsValues[i]
                    });
                }

                // Create Williams %R line series.
                var williamsSeries = new LineSeries
                {
                    ItemsSource = williamsData,
                    XBindingPath = nameof(ChartPoint.Index),
                    YBindingPath = nameof(ChartPoint.Value),
                    StrokeWidth = 2
                };

                chart.Series.Add(williamsSeries);

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

                chart.Annotations.Clear();

                chart.Annotations.Add(new HorizontalLineAnnotation
                {
                    Y1 = -20,
                    Stroke = Colors.Red,  
                    StrokeWidth = 1,
                    Text = "Overbought"
                });

                chart.Annotations.Add(new HorizontalLineAnnotation
                {
                    Y1 = -80,
                    Stroke = Colors.Green,
                    StrokeWidth = 1,
                    Text = "Oversold"
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"Error Plotting Williams: {ex.Message}");
            }
        }


        public static void PlotStochastic(SfCartesianChart chart, string companyName)
        {
            // Make sure company data is loaded.
            LoadCompanyData(companyName, 100);

            try
            {
                // Clear existing chart.
                chart.Series.Clear();

                // Calculate Stochastic Oscillator (%K).
                List<decimal> stochasticValues = TechnicalIndicators.Stochastic(
                    lst_High,
                    lst_Low,
                    lst_Closing,
                    14);

                // Create chart data, skipping warm-up values.
                var stochasticData = new List<ChartPoint>();

                for (int i = 0; i < stochasticValues.Count; i++)
                {
                    if (stochasticValues[i] == decimal.MinValue)
                        continue;

                    stochasticData.Add(new ChartPoint
                    {
                        Index = i + 1,
                        Value = stochasticValues[i]
                    });
                }

                // Create the Stochastic line series.
                var stochasticSeries = new LineSeries
                {
                    ItemsSource = stochasticData,
                    XBindingPath = nameof(ChartPoint.Index),
                    YBindingPath = nameof(ChartPoint.Value),
                    StrokeWidth = 2
                };

                chart.Series.Add(stochasticSeries);

                // Set Y-axis range to 0–100.
                if (chart.YAxes.Count > 0 && chart.YAxes[0] is NumericalAxis yAxis)
                {
                    yAxis.Minimum = 0;
                    yAxis.Maximum = 100;
                    yAxis.Interval = 20;                   
                }

                chart.Annotations.Clear();

                // Clear any previous annotations.
                chart.Annotations.Clear();

                // Overbought line.
                chart.Annotations.Add(new HorizontalLineAnnotation
                {
                    Y1 = 80,
                    Stroke = Colors.Red,
                    StrokeWidth = 1,                    
                    Text = "Overbought"
                    
                });

                // Oversold line.
                chart.Annotations.Add(new HorizontalLineAnnotation
                {
                    Y1 = 20,
                    Stroke = Colors.Green,
                    StrokeWidth = 1,
                    Text = "Oversold"                   
                });

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"Error plotting Stochastic: {ex.Message}");
            }
        }

        public static void PlotMACD(SfCartesianChart chart, string companyName)
        {
            // Load last 100 days of company data.
            LoadCompanyData(companyName, 100);

            try
            {
                chart.Series.Clear();
                chart.Annotations.Clear();

                // Calculate MACD, Signal and Histogram.
                MACDResult macd = TechnicalIndicators.CalculateMACD(lst_Closing);

           


                var macdData = new List<ChartPoint>();
                var signalData = new List<ChartPoint>();
                var histogramData = new List<ChartPoint>();



                for (int i = 0; i < macd.MACD.Count; i++)
                {
                    if (macd.Histogram[i] != decimal.MinValue)
                    {
                        histogramData.Add(new ChartPoint
                        {
                            Index = i + 1,
                            Value = macd.Histogram[i]
                        });
                    }

                    if (macd.MACD[i] != decimal.MinValue)
                    {
                        macdData.Add(new ChartPoint
                        {
                            Index = i + 1,
                            Value = macd.MACD[i]
                        });
                    }

                    if (macd.Signal[i] != decimal.MinValue)
                    {
                        signalData.Add(new ChartPoint
                        {
                            Index = i + 1,
                            Value = macd.Signal[i]
                        });
                    }
                }



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
                    IsVisible = false
                });

                chart.YAxes.Add(new NumericalAxis
                {
                    IsVisible = true
                });
            }

            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"Error plotting MACD: {ex.Message}");
            }
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

        public static string BuyOrSell(string companyName)
        {
            LoadCompanyData(companyName, 100);

            const int period = 30;

            List<decimal> prices = lst_Closing;

            decimal price = prices.Last();

            // Last 30 prices.
            List<decimal> recent = prices.TakeLast(period).ToList();

            // Linear regression slope.
            decimal slope = TechnicalIndicators.LinearRegressionSlope(prices, period);
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

