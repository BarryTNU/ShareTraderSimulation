using Microsoft.Maui.Controls.Handlers;
using ShareTrader.Services;

namespace ShareTrader
{
    public class PortfolioManager

    {
        public static readonly string crlf = Environment.NewLine;
        //   string fPath;

        public const int MaxPortfolioCompanies = 50;

        public static bool success;
        public static async Task UpdatePortfolio()
        {
            try
            {
                //==== Match a file in StockData with a company in MyPortfolio and update dgPortfolio ====

                int SharePackage = 0;//Number of shares in a parcel
                 decimal ParcelCost = 0m;//
                decimal ParcelValue = 0m;//Total value of all shares
                 decimal SharePrice = 0m;//Current share price
                decimal ParcelGain = 0m;//Total gain/loss of all shares
                decimal TradePrice = 0m;//Cost of Purchase
                 string fPath = "";


                AppGlobals.PortfolioItems.Clear();
                AppGlobals.CapitalInvested = 0m;
                AppGlobals.PortfolioValue = 0m;
                bool Success = await FileManager.LoadPortfolio();

                //Iterate through the Portfolio list and get data for each company.

                foreach (string line in AppGlobals.MyPortfolio)
                {
                    try
                    {
                        SharePackage = 0;
                        SharePrice = 0;
                        ParcelValue = 0;
                        ParcelGain = 0m;//Total gain/loss of all shares
                        TradePrice = 0m;//Cost of Purchase
                        TradePrice = 0m; //  Reset Prices for each company

                        if (string.IsNullOrWhiteSpace(line))
                            continue;

                        string[] sp = line.Split(',');

                        string Name = sp[0]; //Get the company name
                        string Symbol = sp[1];

                        //===== Get the closing price for this company=====

                        SharePrice = FileManager.LoadCompanyData(Name, 1); //Closing Price of share


                        List<AppGlobals.TransactionItem> trades =
                        FileManager.LoadTradingHistory(Name);

                        ParcelSummary parcel = CalculateParcelSummary(trades);                     

                        //=======================================================================================================

                       SharePackage = parcel.Shares;
                        ParcelCost = SharePackage * parcel.AverageBuyPrice;
                        ParcelValue =SharePackage * SharePrice;
                        TradePrice = parcel.AverageBuyPrice;
                        ParcelGain = ParcelValue - ParcelCost;

                        AppGlobals.CapitalInvested += ParcelCost;
                        AppGlobals.PortfolioValue += ParcelValue;


                        string trends = ChartManager.BuyOrSell(Name);

                        AppGlobals.PortfolioItems.Add(new AppGlobals.PortfolioItem
                        {
                            CompanyName = Name,
                            Trend = trends,
                            Shares = SharePackage,
                            TradePrice = parcel.AverageBuyPrice,
                            CurrentPrice = SharePrice,
                            ItemCost = ParcelCost,
                            ItemValue = ParcelValue,
                            Profit = ParcelGain
                        });

}
                    catch (Exception ex)
                    {

                        await AppGlobals.ShowMessage(
                        "UpdatePortfolio Error",
                        $"{line}\n\n{ex.Message}");
                        continue;// Optionally log or handle the exception
                    }
                }

                AppGlobals.GainsLosses = AppGlobals.PortfolioValue - AppGlobals.CapitalInvested;

                //Get the Bank Balance>
                if (AppGlobals.BankBalance == 0m)

                    {

                    fPath = AppGlobals.BankBalanceFile;
                    string text = File.ReadAllText(fPath);

                    if (decimal.TryParse(text, out decimal balance))
                    {
                        AppGlobals.BankBalance = balance;
                    }
                    else
                    {
                        AppGlobals.BankBalance = 0m;   // or handle the error
                    }                                                        
                 
                }
            }

            catch (Exception)
            {
                // Optionally log or handle the exception
            }
        
     } 

        //====Add company to Portfolio======
        public static async Task AddSelectedCompany(string CompanyName, string Symbol)
        {

            if (CompanyName == null)
            {
                await AppGlobals.ShowMessage("Portfolio", "Please select a Company.");
                return;
            }

            string fPath = AppGlobals.PortfolioFile;

            // Prevent duplicates
            if (File.Exists(fPath))
            {
                foreach (string line in File.ReadAllLines(fPath))
                {
                    string[] parts = line.Split(',');

                    if (parts.Length > 1 &&
                        parts[0].Equals(CompanyName, StringComparison.OrdinalIgnoreCase))

                    {
                        await AppGlobals.ShowMessage("Portfolio", CompanyName + " is already in your portfolio.");
                        return;
                    }
                }
            }

            //Download share prices for this company

            try
            {
                string provider = AppGlobals.ConfigurationManager.APIProvider;

                success = await Services.DownloadService.DownloadData(Symbol, CompanyName);

            }
            finally
            {
                await AppGlobals.HideLoading();
            }

            if (success)  
            {
                // Write new company
                string record = $"{CompanyName},{Symbol}";

                File.AppendAllText(fPath, record + Environment.NewLine);

                await AppGlobals.ShowMessage("Portfolio", CompanyName + " added to Portfolio.");

                await UpdatePortfolio();

                // Add to Log file.
                string logEntry = $"{CompanyName} added to Portfolio.";
                FileManager.SaveLogFile(logEntry);

            }
            else
            {
                string message = $"Your API Providor may not support this symbol ({Symbol}) or the symbol is invalid. Please check and try again.";
                await AppGlobals.ShowMessage("Download Failed",
                    $"{message}");
                success = false;
            }

            return;
        }

        //===Remove Company from Portfolio=======
        public static async Task RemovePortfolioItem(string companyName, string shares)
        {
            var page = Application.Current?.Windows.FirstOrDefault()?.Page;
            if (page == null)
                return;

            // Confirm deletion.
            bool answer = await page.DisplayAlert(
                "Remove Company",
                $"Are you sure you want to remove {companyName}?",
                "Yes",
                "No");

            if (!answer)
                return;
            //Check if we hold company shares
            shares = (await CheckHoldings(companyName)).ToString();

            // If shares are still held, ask whether to sell them.
            if (shares != "0")
            {
                answer = await page.DisplayAlert(
                    $"You still hold {shares} {companyName} shares.",
                    "Do you want to sell them?",
                    "Yes",
                    "No");

                if (!answer)
                    return;

                decimal SharePrice = FileManager.LoadCompanyData(companyName, 1); //Closing Price of share

                int nrShares = int.Parse(shares);
                await ShareTrading.SellShares(companyName, nrShares,SharePrice );
            }

            // Remove company from portfolio.
            var tempList = new List<string>();

            foreach (var item in AppGlobals.MyPortfolio)
            {
                var spt = item.Split(',');

                if (!spt[0].Equals(companyName, StringComparison.OrdinalIgnoreCase))
                    tempList.Add(item);
            }

            string fPath = AppGlobals.PortfolioFile;

            if (File.Exists(fPath))
            {
                File.WriteAllLines(fPath, tempList); // Re-write the Portfoliofile eithout this company
                 //Delete the company data file
                fPath = Path.Combine(AppGlobals.TradingHistoryPath, companyName + ".csv ");

                if (File.Exists(fPath))
                {
                    File.Delete(fPath);
                }
                //Delete the company tmp data file
                fPath = Path.Combine(AppGlobals.TradingHistoryPath, companyName + ".tmp ");

                if (File.Exists(fPath))
                {
                    File.Delete(fPath);
                }
                // Add to Log file.
                string logEntry = $"{companyName} Deleted ";
                FileManager.SaveLogFile(logEntry);               

            }
          
            await UpdatePortfolio();
        } 
        
        private async static Task<int> CheckHoldings(string company)
        {
            int Holdings = 0;

            List<AppGlobals.TransactionItem> trades =
               FileManager.LoadTradingHistory(company);
            //=====Set up a new list to hold the updated tradingItems list=======
            List<AppGlobals.TransactionItem> tempList = new List<AppGlobals.TransactionItem>();
                      

            foreach (var tradeItem in trades)
            {
                if (tradeItem == null)
                    continue;
                if (tradeItem.Shares == 0)
                    continue;                               

                {
                   Holdings += tradeItem.Shares;
                   
                }                
            }
            
            return Holdings;
        }
        //=====================================================
        public class ParcelSummary
        {
            public int Shares { get; set; }

            public decimal AverageBuyPrice { get; set; }
            public decimal AverageSellPrice { get; set; }

            public decimal TotalInvested { get; set; }
            public decimal TotalSold { get; set; }

            public decimal RealisedProfit { get; set; }
        }

        public static ParcelSummary CalculateParcelSummary(List<AppGlobals.TransactionItem> trades)
        {
            var summary = new ParcelSummary();

            decimal totalBuyCost = 0m;
            int totalBuyShares = 0;

            decimal totalSellValue = 0m;
            int totalSellShares = 0;

            foreach (var trade in trades)
            {
                if (trade == null)
                    continue;

                int shares = trade.Shares;
                decimal price = trade.TradePrice;      // Transaction price

                if (trade.tradeType == "Buy")
                {
                    summary.Shares += shares;

                    totalBuyShares += shares;
                    totalBuyCost += shares * price;
                    if (totalBuyShares > 0) 
                     summary.AverageBuyPrice = totalBuyCost / totalBuyShares;
                     else
                     summary.AverageBuyPrice = price;
                }
               else if (trade.tradeType == "Sell")
              {
                    // Calculate profit before reducing holdings
                   decimal costOfSharesSold = shares * summary.AverageBuyPrice;

                    summary.RealisedProfit += (shares * price) - costOfSharesSold;

                    summary.Shares -= shares;

                    totalSellShares += shares;
                    totalSellValue += shares * price;

                   if (totalSellShares > 0)
                        summary.AverageSellPrice = totalSellValue / totalSellShares;
                   else
                        summary.AverageSellPrice = price;

                    // Remove sold shares from remaining parcel
                   totalBuyCost -= costOfSharesSold;
                  totalBuyShares -= shares;
               }
            }

            summary.TotalInvested = totalBuyCost;
            summary.TotalSold = totalSellValue;
           

            return summary;
        }


    }
}