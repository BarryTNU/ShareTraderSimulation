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

                //   decimal currentPrice = 0;         
                decimal PfCost = 0;
                decimal PfValue = 0;
                decimal PfGain = 0;
                decimal Price = 0;
                decimal bPrice = 0m; // FIX: Initialize bPrice
                int Holdings = 0;
                string fPath = "";
                //    string bS = "";

                AppGlobals.PortfolioItems.Clear();
                AppGlobals.CapitalInvested = 0m;
                AppGlobals.PortfolioValue = 0m;
                bool Success = await FileManager.LoadPortfolio();

                //Iterate through the Portfolio list and get data for each company.

                foreach (string line in AppGlobals.MyPortfolio)
                {
                    try
                    {
                        Holdings = 0;
                        Price = 0;
                        PfValue = 0;
                        PfGain = 0;
                        PfCost = 0;
                        bPrice = 0m; // FIX: Reset bPrice for each company

                        if (string.IsNullOrWhiteSpace(line))
                            continue;

                        string[] sp = line.Split(',');

                        string Name = sp[0]; //Get the company name
                        string Symbol = sp[1];

                        //===== Get the closing price for this company=====

                        decimal SharePrice = FileManager.LoadCompanyData(Name, 1); //Closing Price of share

                        //====== Get the Trading History for this company=====

                        List<AppGlobals.TransactionItem> trades = FileManager.LoadTradingHistory(Name);

                        foreach (AppGlobals.TransactionItem trade in trades)
                        {
                            if (trade == null)
                                continue;

                            Name = trade.Name;
                            int shares = trade.Holdings;
                            bPrice = trade.BuyPrice;
                            Price = trade.BuyPrice * shares;
                            PfCost += Price;
                            Holdings += shares;
                        }

                        PfValue = Holdings * SharePrice;
                        PfGain = PfValue - PfCost;
                        AppGlobals.CapitalInvested += PfCost;
                        AppGlobals.PortfolioValue += PfValue;


                        string trends = ChartManager.BuyOrSell(Name);

                        AppGlobals.PortfolioItems.Add(new AppGlobals.PortfolioItem
                        {
                            CompanyName = Name,
                            Trend = trends,
                            Shares = Holdings,
                            BuyPrice = bPrice,
                            TotalCost = PfCost,
                            CurrentPrice = SharePrice,
                            Value = PfValue,
                            Profit = PfGain
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
                File.WriteAllLines(fPath, tempList);

                string logEntry = $"{DateTime.Today:d} {companyName} Deleted ";
                string logFile = AppGlobals.LogFile;

                File.AppendAllText(logFile, logEntry + Environment.NewLine);
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
                if (tradeItem.Holdings == 0)
                    continue;                               

                {
                   Holdings += tradeItem.Holdings;
                   
                }                
            }

            




            return Holdings;
        }
       
    }
}