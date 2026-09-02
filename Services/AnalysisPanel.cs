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
        public static void DisplayAnalysis(
               SfCartesianChart bollingerChart,
               SfCartesianChart adxChart,
               SfCartesianChart volumeChart,
               string company)
        {
            ChartManager.PlotBollingerBands(bollingerChart, 30, company);
            ChartManager.PlotADX(adxChart, company);
            ChartManager.PlotMAS(volumeChart, company);
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
            var transaction = FileManager.GetTradeInfo(company);

            if (transaction is null)
                return ("", "");

            string row3 =
                $"Purchased {transaction.Holdings} {company} shares for {transaction.BuyPrice:C} on {transaction.TransDate:dd/MM/yyyy}.";

            return (row3, "");
        }






    }




}

    






