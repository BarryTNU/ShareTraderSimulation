
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
          //  string fPath = "";
          //  string tempPath = "";
            string LogData = "";
            string message;
       //    bool response;
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

            string tradeInfo = $"{DateTime.Today:d} {shares} Bought @ {price:C}";            

            string tradeDate = DateTime.Today.ToString("yyyy-MM-dd");
            string tradeData = $"{company},{shares},{price},{tradeDate}";
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
            string tempPath = "";
            string LogData = "";
            int holdings = 0;
            int totalHoldings = 0;
            decimal TotalCost = 0m; // Initialize to avoid CS0165

            List<AppGlobals.TransactionItem> trades =
                FileManager.LoadTradingHistory(company);
            //=====Set up a new list to hold the updated tradingItems list=======
            List<AppGlobals.TransactionItem> tempList = new List<AppGlobals.TransactionItem>();


                //Code here to see if we have enough shares to conduct the trade.
                int remainingToSell = shares;

                foreach (var tradeItem in trades)
                    {
                    if (tradeItem == null)
                        continue;
                    if (tradeItem.Holdings == 0)
                        continue;

                    if (remainingToSell <= 0)
                    {
                        tempList.Add(tradeItem);
                        continue;
                    }

                    if (tradeItem.Holdings <= remainingToSell)
                    {
                        TotalCost += tradeItem.BuyPrice * tradeItem.Holdings;
                        totalHoldings += tradeItem.Holdings;
                        remainingToSell -= tradeItem.Holdings;
                        tradeItem.Holdings = 0;
                    }
                    else
                    {
                        totalHoldings += tradeItem.Holdings;
                        TotalCost += tradeItem.BuyPrice * remainingToSell;
                        tradeItem.Holdings -= remainingToSell;
                        remainingToSell = 0;
                    }

                    tempList.Add(tradeItem);
                }

                if (totalHoldings < shares)
            {
                await AppGlobals.ShowMessage(
                    "Sell Shares",
                    "You have insufficient shares for this trade.");
                return;
            }

            decimal value = shares * price;

            string message =
                $"Selling {shares} {company} shares will return {value:C}";

            var page = Application.Current?.Windows.FirstOrDefault()?.Page;

            if (page == null)
                return;          // or return false if this method returns bool

            bool answer = await page.DisplayAlert(
                "Continue?",
                message,
                "Yes",
                "No");

            if (!answer)
                return;

            holdings -= shares;

            AppGlobals.BankBalance += value;
            AppGlobals.CapitalInvested -= TotalCost;

            
            //====Save the TempList, then delete the old list and replace it with the new list====
             tempPath = Path.Combine(AppGlobals.TradingHistoryPath, company + ".tmp");
             fPath = Path.Combine(AppGlobals.TradingHistoryPath, company + ".csv");
            FileManager.EnsureFolderExists(fPath);

            using (StreamWriter writer = new StreamWriter(tempPath))
            {
                foreach (AppGlobals.TransactionItem tradeItem in tempList)
                {
                    if (tradeItem.Holdings > 0)
                    {
                        writer.WriteLine(
                            $"{tradeItem.Name}," +
                            $"{tradeItem.Holdings}," +
                            $"{tradeItem.BuyPrice}," +
                            $"{tradeItem.TransDate:yyyy-MM-dd}");
                    }
                }
                writer.Close();
                File.Delete(fPath);          
                File.Copy(tempPath, fPath);
             }

            LogData = $" Sold {shares} {company} Shares @ {price:C}";

            FileManager.SaveLogFile(LogData);
            FileManager.SaveConfig();
            FileManager.SaveBalances();
           
        }
    }
}
            
         
    
    





