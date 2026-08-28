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
            string Message = $@"This will clear all data except company Trade data.";
            bool answer = await Application.Current.MainPage.DisplayAlert(
                      "Reset all Data. are you sure?",
                        Message,
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
               // File.Delete(AppGlobals.InvestmentFile);
                File.Delete(AppGlobals.LogFile);

            }
        }
    }
}
