using System.Globalization;
using Syncfusion.Maui.Charts;
using System.Collections.ObjectModel;

namespace ShareTrader.Services
{
    public class AnalysisResult
    {
        public string Recommendation { get; set; } = "";
        public string Row1 { get; set; } = "";
        public string Row2 { get; set; } = "";
        public string Row3 { get; set; } = "";
        public string Row4 { get; set; } = "";

        public ObservableCollection<string> PriceList { get; set; } = new();
    }


    public class AnalysisPanel
    {
      
        public static void PlotChart(string chartType,
                                   SfCartesianChart chart,
                                   string company)
        {
            switch (chartType)
            {
                case "ADX":
                ChartManager.PlotADX(chart, company);
                    break;

                case "BOLL":
                    ChartManager.PlotBollingerBands(chart, 30, company);
                    break;

                case "LRS":
                    ChartManager.PlotLRS(chart,company);
                    break;

                case "MAS":
                    ChartManager.PlotMAS(chart, company);
                    break;

                case "MACD":
                    ChartManager.PlotMACD(chart, company);
                    break;

                case "RSI":
                      ChartManager.PlotRSI(chart, company);
                    break;

                case "STOC":
                    ChartManager.PlotStochastic(chart, company);
                    break;

                case "WILL":
                    ChartManager.PlotWilliams(chart, company);
                    break;

                case "ROC":
                    ChartManager.PlotROC(chart, company);
                    break;

                case "ATR":
                    ChartManager.PlotATR(chart, company);
                    break;

                case "VOL":
                    ChartManager.PlotVolume(chart, company);
                    break;

                case "OBV":
                    ChartManager.PlotOBV(chart, company);
                    break;

                default:
                    chart.Series.Clear();
                    break;
            }
        }

        public static string GetRecommendation(string company)
        {
            string signal = ChartManager.BuyOrSell(company);

            return signal switch
            {
                "▲ ▲ ▲" => TextFiles.StrongBuy(),
                "▲ ▲" => TextFiles.MediumBuy(),
                "▲" => TextFiles.Buy(),
                "▶" => TextFiles.Hold(),
                "▼" => TextFiles.Sell(),
                "▼ ▼" => TextFiles.MediumSell(),
                "▼ ▼ ▼" => TextFiles.StrongSell(),
                _ => ""
            };
        }
        public static (string Low, string High) GetPriceSummary()
        {
            string low =
                $"Low {AppGlobals.PeriodLow:C} on {AppGlobals.Lowdate:dd/MM/yyyy}";

            string high =
                $"High {AppGlobals.PeriodHigh:C} on {AppGlobals.Highdate:dd/MM/yyyy}";

            return (low, high);
        }

        public static (string Row3, string Row4) GetPurchaseSummary(string company)
        {
            //Possibly expand the purchase summary to include multiple purchases in the future, but for now just return the last purchase.

            var transaction = FileManager.GetTradeInfo(company);

            if (transaction is null)
                return ("", "");

            string row3 =
                $"Purchased {transaction.Shares} {company} shares for {transaction.TradePrice:C} on {transaction.TransDate:dd/MM/yyyy}.";

            return (row3, "");
        }






    }




}

    






