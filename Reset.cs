using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShareTrader.Services;
namespace ShareTrader

{
    public class Reset
    {
     public async static void ResetAlldData()
        {
            string message = "This will clear all data except company Trade data.";

            var page = Application.Current?.Windows.FirstOrDefault()?.Page;

            if (page == null)
                return;          // or return false if this method returns bool

            bool answer = await page.DisplayAlert(
                "Reset all Data. Are you sure?",
                message,
                "Yes",
                "No"); 
            // Only proceed if user clicked Yes
            if (!answer)
            {
                return; // User clicked No
            }
            else         // User clicked Yes
            {
                File.Delete(AppGlobals.ConfigFile);
                File.Delete(AppGlobals.CompaniesFile);
                File.Delete(AppGlobals.PortfolioFile);
                File.Delete(AppGlobals.BankBalanceFile);
                File.Delete(AppGlobals.LastPriceUpdate);
                File.Delete(AppGlobals.PortfolioPath + "\\Capitalinvested.csv");
                Directory.Delete(AppGlobals.PortfolioPath +"\\TradingHistory", true);

               await PortfolioManager.UpdatePortfolio();
          }
       }
    }
}
