using ShareTrader.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ShareTrader
{
    
    public  class ShareTrading       

    {
     
            public static decimal CalculateTradeValue(string? text, decimal currentPrice)
            {
                if (string.IsNullOrWhiteSpace(text))
                    return 0m;

                if (!int.TryParse(text, out int shares))
                    return -1m;          // Invalid input

                return shares * currentPrice;
            }
      


        public static async Task BuyShares(string company, int shares, decimal price)
         {
             string LogData = "";
            string message;
             decimal BankBalance = 0m;

            decimal value = shares * price;

            string bPath = AppGlobals.BankBalanceFile;

            if (File.Exists(bPath))
                BankBalance = decimal.Parse(File.ReadAllText(bPath));

            if (BankBalance < value)
            {
                await AppGlobals.ShowMessage(
                    "Portfolio",
                    "You have insufficient funds for this trade.");

                return;
            }

            message = $"Buying {shares} {company} shares will cost {value:C}";

            var page = Application.Current?.Windows.FirstOrDefault()?.Page;

            if (page == null)
                return;          // or return false if this method returns bool

            bool answer = await page.DisplayAlert(
                "Confirm Purchase?",
                message,
                "Yes",
                "No");

            if (!answer)
                return;


            AppGlobals.BankBalance -= value;
            AppGlobals.CapitalInvested += value;
            string tradeType = "Buy";

            string tradeInfo = $"{DateTime.Today:d} {shares} Bought @ {price:C}";            

            string tradeDate = DateTime.Today.ToString("yyyy-MM-dd");
            string tradeData = $"{company},{shares},{price},{tradeDate},{tradeType},{value}";
            LogData = $" Brought {shares} {company} Shares @ {price:C}";                  

            FileManager.SaveTradingHistory(company, tradeData);
            FileManager.SaveLogFile(LogData);
            FileManager.SaveConfig();
            FileManager.SaveBalances();

           await PortfolioManager.UpdatePortfolio();
        }

        public static async Task SellShares(string company, int shares, decimal price)
        {
            string fPath = "";
            string LogData = "";
            int totalHoldings = 0;          
            decimal sharePrice = 0m;
            int nrShares = 0;
            int available = 0;
            string BuySell = "";
            decimal SaleProceeds = 0m;           
           
            DateOnly DateToday = DateOnly.FromDateTime(DateTime.Today);

            int remainingToSell = shares;

            List<AppGlobals.TransactionItem> trades =
                FileManager.LoadTradingHistory(company);
            // =====Set up a new list to hold the updated tradingItems list=======
            List<AppGlobals.TransactionItem> tempList = new List<AppGlobals.TransactionItem>();

            // Code here to see if we have enough shares to conduct the trade.
            foreach (AppGlobals.TransactionItem trade in trades)
            {
                nrShares = trade.Shares;
                BuySell = (trade.tradeType ?? "").Trim();
                if (BuySell == "Buy" || BuySell == "") // catches old records that have no tradeType set
                {
                    totalHoldings += nrShares;
                }
                else if (BuySell == "Sell")
                {
                    totalHoldings -= nrShares;
                }
            }

            if (totalHoldings < shares)
            {
                await AppGlobals.ShowMessage(
                    "Sell Shares",
                    "You have insufficient shares for this trade.");
                return;
            }
            //===== Get the closing price for this company=====

            sharePrice = FileManager.LoadCompanyData(company, 1); //Closing Price of share
            
            decimal value = shares * price; // Calculate the value of the shares being sold using the current price

            string message =
                $"Selling {shares} {company} shares will return {value:C}";

            var page = Application.Current?.Windows.FirstOrDefault()?.Page;

            if (page == null)
                return;

            bool answer = await page.DisplayAlert(
                "Continue?",
                message,
                "Yes",
                "No");

            if (!answer)
                return;

            //====================================================================
            foreach (var tradeItem in trades)
            {
                if (tradeItem == null)
                    continue;
               

                if (tradeItem.Shares == 0)
                    continue;

                
                decimal tradePrice = tradeItem.TradePrice;

                if (remainingToSell <= 0)
                {
                    // no more to sell, keep the remaining buy record as-is
                    tempList.Add(tradeItem);
                    continue;
                }

                BuySell = (tradeItem.tradeType ?? "").Trim();
                if (BuySell == "Sell")// skip sell records
                    continue;

                available = tradeItem.Shares;

                if (available <=  remainingToSell)
                {
                    //consume this buy record
                     remainingToSell -=available;                   
                    SaleProceeds += tradePrice * available;
                    available = 0;                 
                    
                }
                else // available >= remainingToSell
                {
                    // Partialy consume the buy record
                    available -= remainingToSell;
                    SaleProceeds += tradePrice * remainingToSell;
                    remainingToSell =0;                    
                }

                // add updated buy record with remaining shares
                var updated = new AppGlobals.TransactionItem
                {
                    Name = tradeItem.Name,
                    Shares = available,
                    TradePrice = tradePrice,
                    TransDate = DateToday,
                    tradeType = "Buy"
                };

                tempList.Add(updated);
            }


            fPath = Path.Combine(AppGlobals.TradingHistoryPath, company + ".csv");
            FileManager.EnsureFolderExists(fPath);

            // Write the updated buy records back to file (tempList)
            using (StreamWriter writer = new StreamWriter(fPath))
            {
                foreach (AppGlobals.TransactionItem ti in tempList)
                {
                    if (ti.Shares > 0) // Only write the tradeItem to the file if it has shares greater than 0
                    {
                        writer.WriteLine(
                            $"{ti.Name}," +
                            $"{ti.Shares}," +
                            $"{ti.TradePrice}," +
                            $"{ti.TransDate:yyyy-MM-dd}," +
                            $"{ti.tradeType}");
                    }
                }
                writer.Close();
            }

            LogData = $" Sold {shares} {company} Shares @ {price:C}";
             FileManager.SaveLogFile(LogData);
            FileManager.SaveConfig();
            FileManager.SaveBalances();

            await PortfolioManager.UpdatePortfolio();
        }       
    }
}
            
         
    
    









