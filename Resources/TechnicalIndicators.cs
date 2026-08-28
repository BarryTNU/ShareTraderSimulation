using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareTrader
{
           public static class TechnicalIndicators
        {
            public static float LinearRegressionSlope(List<float> prices, int period)
            {
                if (prices.Count < period)
                    return 0f;

                if (prices.Count < period)
                    period = prices.Count - 1;

                int startIndex = prices.Count - period;

                int n = period;

                float sumX = 0f;
                float sumY = 0f;
                float sumXY = 0f;
                float sumX2 = 0f;

                for (int i = 0; i < n; i++)
                {
                    float x = i;
                    float y = prices[startIndex + i];

                    sumX += x;
                    sumY += y;
                    sumXY += x * y;
                    sumX2 += x * x;
                }

                float numerator = (n * sumXY) - (sumX * sumY);
                float denominator = (n * sumX2) - (sumX * sumX);

                if (denominator == 0f)
                    return 0f;

                return numerator / denominator;
            }



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

            public static List<decimal> CalculateMACD(
            List<decimal> prices,
            int fastPeriod,
            int slowPeriod)
            {
                List<decimal> fastEMA = CalculateEMA(prices, fastPeriod);
                List<decimal> slowEMA = CalculateEMA(prices, slowPeriod);

                List<decimal> macd = new List<decimal>(prices.Count);

                for (int i = 0; i < prices.Count; i++)
                {
                    if (fastEMA[i] == decimal.MinValue ||
                        slowEMA[i] == decimal.MinValue)
                    {
                        macd.Add(decimal.MinValue);
                    }
                    else
                    {
                        macd.Add(fastEMA[i] - slowEMA[i]);
                    }
                }

                return macd;
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
                    if (macd[i] == decimal.MinValue ||
                        signal[i] == decimal.MinValue)
                    {
                        histogram.Add(decimal.MinValue);
                    }
                    else
                    {
                        histogram.Add(macd[i] - signal[i]);
                    }
                }

                return histogram;
            }
            public static List<decimal> CalculateEMA(List<decimal> values, int period)
            {
                List<decimal> result = new List<decimal>(values.Count);

                decimal multiplier = 2m / (period + 1);

                int firstValidIndex = -1;

                // Initialize result with invalid values
                for (int i = 0; i < values.Count; i++)
                {
                    result.Add(decimal.MinValue);
                }

                // Find the first valid value
                for (int i = 0; i < values.Count; i++)
                {
                    if (values[i] != decimal.MinValue)
                    {
                        firstValidIndex = i;
                        break;
                    }
                }

                // No valid values
                if (firstValidIndex == -1)
                    return result;

                // Seed EMA with first valid value
                result[firstValidIndex] = values[firstValidIndex];

                // Compute EMA
                for (int i = firstValidIndex + 1; i < values.Count; i++)
                {
                    if (values[i] == decimal.MinValue)
                    {
                        // Carry previous EMA forward
                        result[i] = result[i - 1];
                    }
                    else
                    {
                        result[i] = (values[i] - result[i - 1]) * multiplier + result[i - 1];
                    }
                }

                return result;
            }
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

            public static float LinearRegressionSlope(List<float> values)
            {
                int n = values.Count;

                if (n < 2)
                    return 0f;

                float sumX = 0;
                float sumY = 0;
                float sumXY = 0;
                float sumXX = 0;

                for (int i = 0; i < n; i++)
                {
                    sumX += i;
                    sumY += values[i];
                    sumXY += i * values[i];
                    sumXX += i * i;
                }

                return (n * sumXY - sumX * sumY) /
                       (n * sumXX - sumX * sumX);
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


        }
    }



