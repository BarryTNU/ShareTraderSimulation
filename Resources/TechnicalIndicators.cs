using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareTrader;

     public class MACDResult
{
    public List<decimal> MACD { get; set; } = new();
    public List<decimal> Signal { get; set; } = new();
    public List<decimal> Histogram { get; set; } = new();
}


    public static class TechnicalIndicators
    {
         public static List<decimal> sma(List<decimal> prices, int period)
        {
            List<decimal> result = new List<decimal>();

            if (prices == null || prices.Count == 0)
                return result;

            if (prices.Count < period)
                period = prices.Count;

            int startIndex = period - 1;

            for (int i = startIndex; i < prices.Count; i++)
            {
                decimal average = prices
                    .Skip(i - period + 1)
                    .Take(period)
                    .Average();

                result.Add(average);
            }

            return result;
        }

        public static List<decimal> SMA(List<decimal> prices, int period)
        {
            List<decimal> lst_sma = new List<decimal>();

            for (int i = 0; i < prices.Count; i++) // Removed stray semicolon and fixed loop range
            {
                if (i < period - 1)
                {
                    lst_sma.Add(decimal.MinValue);   // Placeholder (decimal has no NaN)
                }
                else
                {
                    decimal total = 0m;

                    for (int j = i - period + 1; j <= i; j++)
                    {
                        total += prices[j];
                    }

                    lst_sma.Add(total / period);
                }
            }

            return lst_sma;
        }


        public static List<decimal> PlotMovingAverageSlope(List<decimal> ma, int slopePeriod, int maPeriod)
        {
            List<decimal> result = new List<decimal>(ma.Count);

            if (slopePeriod <= 0)
            {
                for (int i = 0; i < ma.Count; i++)
                    result.Add(0m);
                return result;
            }

            for (int i = 0; i < ma.Count; i++)
            {
                if (i < slopePeriod ||
                    ma[i] == decimal.MinValue ||
                    ma[i - slopePeriod] == decimal.MinValue)
                {
                    result.Add(0m);
                }
                else
                {
                    decimal slope = (ma[i] - ma[i - slopePeriod]) / slopePeriod;
                    result.Add(slope);
                }
            }

            return result;
        }

    //==============================================================
    //  MACD ROUTINES
    //==============================================================
    public static MACDResult CalculateMACD(
List<decimal> prices,
int fastPeriod = 12,
int slowPeriod = 26,
int signalPeriod = 9)
    {
        var result = new MACDResult();

        if (prices == null || prices.Count == 0)
            return result;

        // Existing EMA routine.
        List<decimal> fastEMA = CalculateEMA(prices, fastPeriod);
        List<decimal> slowEMA = CalculateEMA(prices, slowPeriod);

        // --- MACD Line ---
        for (int i = 0; i < prices.Count; i++)
        {
            if (fastEMA[i] == decimal.MinValue ||
                slowEMA[i] == decimal.MinValue)
            {
                result.MACD.Add(decimal.MinValue);
            }
            else
            {
                result.MACD.Add(fastEMA[i] - slowEMA[i]);
            }
        }
        // --- Signal Line (EMA of MACD) ---
        // --- Signal Line (EMA of MACD) ---
        result.Signal = CalculateMACDSignal(result.MACD, signalPeriod);

        // --- Histogram ---
        result.Histogram = new List<decimal>(prices.Count);

        for (int i = 0; i < prices.Count; i++)
        {
            if (result.MACD[i] == decimal.MinValue ||
                result.Signal[i] == decimal.MinValue)
            {
                result.Histogram.Add(decimal.MinValue);
                
            }
            else
            {
                result.Histogram.Add(result.MACD[i] - result.Signal[i]);
            }

        }
        return result;
    }


        public static List<decimal> CalculateMACDSignal(
      List<decimal> macd,
      int signalPeriod)
        {
            return CalculateEMA(macd, signalPeriod);
        }

    public static List<decimal> CalculateMACDHistogram(
List<decimal> macd,
List<decimal> signal)
    {
        List<decimal> histogram = new List<decimal>(macd.Count);

        for (int i = 0; i < macd.Count; i++)
        {
            // Reject any invalid values.
            if (macd[i] == decimal.MinValue ||
                signal[i] == decimal.MinValue ||
                macd[i] == decimal.MaxValue ||
                signal[i] == decimal.MaxValue)
            {
                histogram.Add(decimal.MinValue);
                continue;
            }

            histogram.Add(macd[i] - signal[i]);
        }

        return histogram;
    }

    public static List<decimal> LinearRegressionSlope(
    List<decimal> prices,
    int period = 30)
    {
        var result = new List<decimal>();

        // Fill warm-up period.
        for (int i = 0; i < period - 1; i++)
            result.Add(decimal.MinValue);

        for (int end = period - 1; end < prices.Count; end++)
        {
            decimal sumX = 0;
            decimal sumY = 0;
            decimal sumXY = 0;
            decimal sumXX = 0;

            for (int j = 0; j < period; j++)
            {
                decimal x = j;
                decimal y = prices[end - period + 1 + j];

                sumX += x;
                sumY += y;
                sumXY += x * y;
                sumXX += x * x;
            }

            decimal n = period;

            decimal numerator =
                n * sumXY - sumX * sumY;

            decimal denominator =
                n * sumXX - sumX * sumX;

            decimal slope = denominator == 0
                ? 0
                : numerator / denominator;

            result.Add(slope);
        }

        return result;
    }

    // ======================================================
    // Current Linear Regression Slope
    // Returns the most recent LRS value.
    // Used for Buy/Sell calculations.
    // ======================================================
    public static decimal CurrentLinearRegressionSlope(
        List<decimal> prices,
        int period = 30)
    {
        List<decimal> values = LinearRegressionSlope(prices, period);

        return values.Last(v => v != decimal.MinValue);
    }



    public static List<decimal> CalculateEMA(List<decimal> values, int period)
        {
            List<decimal> ema = new List<decimal>(values.Count);

            // Fill result with invalid values.
            for (int i = 0; i < values.Count; i++)
                ema.Add(decimal.MinValue);

            if (values == null || values.Count < period)
                return ema;

            decimal multiplier = 2m / (period + 1);

            // Find first run of valid values long enough for an SMA.
            int start = -1;

            for (int i = 0; i <= values.Count - period; i++)
            {
                bool valid = true;

                for (int j = 0; j < period; j++)
                {
                    if (values[i + j] == decimal.MinValue)
                    {
                        valid = false;
                        break;
                    }
                }

                if (valid)
                {
                    start = i;
                    break;
                }
            }

            if (start == -1)
                return ema;

            // Seed EMA with the SMA of the first period values.
            decimal sma = 0m;
            for (int i = start; i < start + period; i++)
                sma += values[i];

            sma /= period;

            int firstEMA = start + period - 1;
            ema[firstEMA] = sma;

            // Continue EMA calculation.
            for (int i = firstEMA + 1; i < values.Count; i++)
            {
                if (values[i] == decimal.MinValue)
                {
                    ema[i] = ema[i - 1];
                }
                else
                {
                    ema[i] = ema[i - 1] + (values[i] - ema[i - 1]) * multiplier;
                }
            }

            return ema;
        }

        //====================================================
        //   END OF MACD ROUTINES
        //====================================================



        // === RSI FUNCTION ===
        // Relative Strength Index

        public static List<decimal> RSI(List<decimal> values, int period)
        {
            List<decimal> result = new List<decimal>();
            List<decimal> gains = new List<decimal>();
            List<decimal> losses = new List<decimal>();

            if (values == null || values.Count <= period)
                return result;

            for (int i = 1; i < values.Count; i++)
            {
                decimal change = values[i] - values[i - 1];

                gains.Add(Math.Max(change, 0m));
                losses.Add(Math.Max(-change, 0m));
            }

            decimal avgGain = gains.Take(period).Average();
            decimal avgLoss = losses.Take(period).Average();

            for (int i = 0; i < gains.Count; i++)
            {
                avgGain = ((avgGain * (period - 1)) + gains[i]) / period;
                avgLoss = ((avgLoss * (period - 1)) + losses[i]) / period;

                decimal rs = (avgLoss == 0m)
                    ? 100m
                    : avgGain / avgLoss;

                decimal rsiValue = 100m - (100m / (1m + rs));

                result.Add(rsiValue);
            }

            return result;
        }
        // ADX (Average Directional Index)
        // Purpose: Shows strength of a trend.

        public static List<decimal> ADX(
            List<decimal> high,
            List<decimal> low,
            List<decimal> close,
            int period)
        {
            List<decimal> result = new List<decimal>();

            if (high == null || low == null || close == null)
                return result;

            // Create a time-aligned result with the same length as the price list.
            for (int i = 0; i < close.Count; i++)
                result.Add(0m);

            // Need at least period + 1 values.
            if (high.Count <= period ||
                low.Count <= period ||
                close.Count <= period)
                return result;

            List<decimal> tr = new List<decimal>();
            List<decimal> plusDM = new List<decimal>();
            List<decimal> minusDM = new List<decimal>();

            for (int i = 1; i < high.Count; i++)
            {
                decimal trueRange = Math.Max(
                    high[i] - low[i],
                    Math.Max(
                        Math.Abs(high[i] - close[i - 1]),
                        Math.Abs(low[i] - close[i - 1])));

                tr.Add(trueRange);

                decimal upMove = high[i] - high[i - 1];
                decimal downMove = low[i - 1] - low[i];

                plusDM.Add((upMove > downMove && upMove > 0m) ? upMove : 0m);
                minusDM.Add((downMove > upMove && downMove > 0m) ? downMove : 0m);
            }

            // tr[k] corresponds to close[k+1]
            for (int i = period; i < tr.Count; i++)
            {
                decimal atr = tr
                    .Skip(i - period)
                    .Take(period)
                    .Average();

                if (atr == 0m)
                {
                    result[i + 1] = 0m;
                    continue;
                }

                decimal pDI =
                    100m *
                    (plusDM.Skip(i - period).Take(period).Sum() / atr);

                decimal mDI =
                    100m *
                    (minusDM.Skip(i - period).Take(period).Sum() / atr);

                decimal denom = pDI + mDI;

                decimal dx =
                    (denom == 0m)
                        ? 0m
                        : 100m * Math.Abs(pDI - mDI) / denom;

                result[i + 1] = dx;
            }

            return result;
        }

       


        public static List<decimal> CalculateMASlope(List<decimal> ma)
        {
            List<decimal> slope = new List<decimal>();

            if (ma.Count == 0)
                return slope;

            slope.Add(0m);

            for (int i = 1; i < ma.Count; i++)
            {
                slope.Add(ma[i] - ma[i - 1]);
            }

            return slope;
        }


        public static List<decimal> PlotLRS(List<decimal> prices, int period)
        {
            var trendValues = new List<decimal>();

            if (prices == null || prices.Count < 2)
                return trendValues;

            if (prices.Count < period)
                period = prices.Count - 1;

            int startIndex = prices.Count - period;

            for (int i = startIndex; i < prices.Count; i++)
            {
                decimal slope = (prices[i] - prices[i - period]) / prices[i - period];
                decimal y;

                if (slope > 0.02m)
                {
                    y = 1m;      // Up
                }
                else if (slope < -0.02m)
                {
                    y = -1m;     // Down
                }
                else
                {
                    y = 0.02m;      // Sideways
                }

                    trendValues.Add(y);
                }

                return trendValues;
            }
       

        //  ' Stochastic Oscillator

        //  'Purpose: Measures momentum(overbought / oversold).
        // 'Formula:
        // ' %K = (Close – LowestLow) / (HighestHigh – LowestLow) * 100 %D = SMA(%K, 3)
        public static List<decimal> Stochastic(
     List<decimal> high,
     List<decimal> low,
     List<decimal> close,
      int period)
        {
           List<decimal> result = new();

            for (int i = period; i < close.Count; i++)
            {
                decimal hh = high.Skip(i - period).Take(period).Max();
                decimal ll = low.Skip(i - period).Take(period).Min();

               decimal value = ((close[i] - ll) / (hh - ll)) * 100m;
                result.Add(value);
            }

            return result;
        }


        // ' ROC (Rate of Change)
        ////  'Purpose: Measures speed Of price change.
        //  'Formula:
        //  ' ROC = [(Close_today – Close_n_days_ago) / Close_n_days_ago] * 100
        public static List<decimal> ROC(List<decimal> values, int period)
        {
           List<decimal> result = new();

            for (int i = period; i < values.Count; i++)
            {
                decimal roc = ((values[i] - values[i - period]) / values[i - period]) * 100m;
                result.Add(roc);
            }

            return result;
        }



        //  'CCI (Commodity Channel Index)
        //  'Purpose: Detect overbought/oversold levels.

        //  'Formula:
        //  ' CCI = (TypicalPrice – SMA(TypicalPrice)) / (0.015 * MeanDeviation)
        //   'TypicalPrice = (High + Low + Close) / 3
        public static List<decimal> CCI(
   List<decimal> high,
   List<decimal> low,
   List<decimal> close,
    int period)
        {
           List<decimal> result = new();

            for (int i = period; i < close.Count; i++)
            {
               List<decimal> tpList = high.Skip(i - period).Take(period)
                    .Zip(low.Skip(i - period).Take(period), (h, l) => (h + l) / 2m)
                    .Zip(close.Skip(i - period).Take(period), (hm, c) => (hm + c) / 2m)
                    .ToList();

               decimal sma = tpList.Average();

                decimal md = tpList.Average(x => Math.Abs(x - sma));

                decimal index = (tpList.Last() - sma) / (0.015m * md);

                result.Add(index);
            }

            return result;
        }

        //  ' ATR (Average True Range)

        //  'Purpose: Measures volatility.
        //  'Formula:
        //  ' TR = Max(High–Low, |High–PrevClose|, |Low–PrevClose|)
        //  'ATR = SMA(TR, N)

        public static List<decimal> ATR(
   List<decimal> high,
   List<decimal> low,
   List<decimal> close,
    int period)
        {
           List<decimal> tr = new();

            // Calculate True Range
            for (int i = 1; i < high.Count; i++)
            {
                decimal trueRange = Math.Max(
                    high[i] - low[i],
                    Math.Max(
                        Math.Abs(high[i] - close[i - 1]),
                        Math.Abs(low[i] - close[i - 1]))
                );

                tr.Add(trueRange);
            }

           List<decimal> result = new();

            // Calculate ATR as a simple moving average of True Range
            for (int i = period; i < tr.Count; i++)
            {
                decimal average = tr.Skip(i - period).Take(period).Average();
                result.Add(average);
            }

            return result;
        }



        public static List<decimal> OBV(List<decimal> close,List<decimal> volume)
        {
           List<decimal> result = new();
            decimal obvValues = 0;

            for (int i = 1; i < close.Count; i++)
            {
                if (close[i] > close[i - 1])
                {
                    obvValues += volume[i];
                }
                else if (close[i] < close[i - 1])
                {
                    obvValues -= volume[i];
                }

                result.Add(obvValues);
            }

            return result;
        }

        public static List<decimal> PSAR(
   List<decimal> highPrices,
   List<decimal> lowPrices,
    decimal startAF,
    decimal stepAF,
    decimal maxAF)
        {
            int n = highPrices.Count;
           List<decimal> psar = new(new decimal[n]);

            bool trendUp = true;          // Start with an uptrend
            decimal af = startAF;

            // Extreme Point
            decimal ep = lowPrices[0];

            // Initialize PSAR
            psar[0] = lowPrices[0];

            for (int i = 1; i < n; i++)
            {
                decimal priorPSAR = psar[i - 1];
                decimal newPSAR;

                if (trendUp)
                {
                    // Calculate PSAR for an uptrend
                    newPSAR = priorPSAR + af * (ep - priorPSAR);

                    // Stop and reverse
                    if (lowPrices[i] < newPSAR)
                    {
                        trendUp = false;
                        newPSAR = ep;              // Reset PSAR
                        ep = highPrices[i];        // New extreme point
                        af = startAF;
                    }
                    else
                    {
                        // Continue uptrend
                        if (highPrices[i] > ep)
                        {
                            ep = highPrices[i];
                            af = Math.Min(af + stepAF, maxAF);
                        }
                    }
                }
                else
                {
                    // Calculate PSAR for a downtrend
                    newPSAR = priorPSAR - af * (priorPSAR - ep);

                    // Stop and reverse
                    if (highPrices[i] > newPSAR)
                    {
                        trendUp = true;
                        newPSAR = ep;
                        ep = lowPrices[i];
                        af = startAF;
                    }
                    else
                    {
                        // Continue downtrend
                        if (lowPrices[i] < ep)
                        {
                            ep = lowPrices[i];
                            af = Math.Min(af + stepAF, maxAF);
                        }
                    }
                }

                psar[i] = newPSAR;
            }

            return psar;
        }

        // ===================== Williams %R =====================
        // Williams %R is a momentum oscillator that measures how close the
        // current price is to the recent highest high and lowest low.
        // Purpose: Identifies overbought / oversold conditions.

        public static List<decimal> WilliamsR(
            List<decimal> highs,
            List<decimal> lows,
            List<decimal> closes,
            int period)
        {
            var wr = new List<decimal>();

            for (int i = 0; i < closes.Count; i++)
            {
                if (i < period - 1)
                {
                    // Not enough data yet.
                    wr.Add(decimal.MinValue);   // ShareTrader uses this instead of NaN.
                    continue;
                }

                decimal highestHigh = highs.Skip(i - period + 1).Take(period).Max();
                decimal lowestLow = lows.Skip(i - period + 1).Take(period).Min();

                decimal range = highestHigh - lowestLow;

                if (range == 0)
                {
                    wr.Add(0m);
                }
                else
                {
                    decimal value = ((highestHigh - closes[i]) / range) * -100m;
                    wr.Add(value);
                }
            }

            return wr;
        }




        //  '========Volume Expansion (Breakout Strength)=======
        //  '==Call it like  " Dim volExpand = DetectVolumeExpansion(Volume, 20, 1.5)" ===
        public static List<bool> DetectVolumeExpansion(
   List<decimal> volume,
    int period,
    decimal multiplier)
        {
            List<bool> result = new();

            for (int i = 0; i < volume.Count; i++)
            {
                if (i < period)
                {
                    result.Add(false);
                }
                else
                {
                    decimal avg = 0;

                    for (int j = i - period; j <= i - 1; j++)
                    {
                        avg += volume[j];
                    }

                    avg /= period;

                    result.Add(volume[i] > avg * multiplier);
                }
            }

            return result;
        }

    }



