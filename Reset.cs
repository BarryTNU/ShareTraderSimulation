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
                try
                {
                    if (File.Exists(AppGlobals.ConfigFile))
                        File.Delete(AppGlobals.ConfigFile);

                    if (File.Exists(AppGlobals.CompaniesFile))
                        File.Delete(AppGlobals.CompaniesFile);

                    if (File.Exists(AppGlobals.PortfolioFile))
                        File.Delete(AppGlobals.PortfolioFile);

                    if (File.Exists(AppGlobals.BankBalanceFile))
                        File.Delete(AppGlobals.BankBalanceFile);

                    if (File.Exists(AppGlobals.LastPriceUpdate))
                        File.Delete(AppGlobals.LastPriceUpdate);

                    string capitalFile = Path.Combine(AppGlobals.PortfolioPath, "CapitalInvested.csv");
                    if (File.Exists(capitalFile))
                        File.Delete(capitalFile);

                    string historyFolder = Path.Combine(AppGlobals.PortfolioPath, "TradingHistory");

                    if (Directory.Exists(historyFolder))
                    {
                        foreach (string file in Directory.GetFiles(historyFolder))
                        {
                            File.Delete(file);
                        }

                        // Optional: delete any subfolders too.
                        foreach (string dir in Directory.GetDirectories(historyFolder))
                        {
                            Directory.Delete(dir, true);
                        }
                    }              
                }

                catch (Exception ex)
                {
                    await page.DisplayAlert("Error", $"An error occurred while resetting data: {ex.Message}", "OK");
                }
            }
        }
    }
}
