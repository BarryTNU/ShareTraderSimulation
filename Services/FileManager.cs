//using Android.App.Job;
//using Android.Content.Res;
//using Android.Content.Res;
//using Android.Hardware.Camera2;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareTrader.Services
{
    class FileManager

    {

        public static readonly string crlf = Environment.NewLine;

        public static void EnsureFolderExists(string fPath)
        {
            string? folder = Path.GetDirectoryName(fPath);

            if (!string.IsNullOrEmpty(folder))
            {
                Directory.CreateDirectory(folder);
            }
        }


        public static bool LoadConfigData()
        {
            string fPath = "";
            bool Success = false;

            try
            {

                //==========================================
                // Load API Settings
                //==========================================
                fPath = AppGlobals.ConfigFile;

                if (System.IO.File.Exists(fPath))
                {
                    string Data = System.IO.File.ReadAllText(fPath);
                    string[] sp = Data.Split(',');

                    AppGlobals.ConfigurationManager.APIProvider = sp[0];
                    AppGlobals.ConfigurationManager.APIKey = sp[1];
                    AppGlobals.ConfigurationManager.MaxHourlyRequests = int.Parse(sp[2]);
                    AppGlobals.ConfigurationManager.MaxDailyRequests = int.Parse(sp[3]);
                    AppGlobals.ConfigurationManager.RequestCount = int.Parse(sp[4]);
                    AppGlobals.ConfigurationManager.LastUpdate = DateTime.Parse(sp[5]);
                   // AppGlobals.APIProvider = sp[0];
                    Success = true;
                }
            }
            catch (Exception)
            {
                Success = false;
            }

            return Success;
        }

        public static bool LoadBalances()
        {
            string fPath = "";
            bool Success = false;

            try
            {

                {

                    //==========================================
                    // Load Bank Balance
                    //==========================================
                    fPath = AppGlobals.BankBalanceFile;

                    if (System.IO.File.Exists(fPath))
                    {
                        AppGlobals.BankBalance = decimal.Parse(System.IO.File.ReadAllText(fPath), CultureInfo.InvariantCulture);

                        Success = true;
                    }

                }
            }
            catch (Exception)
            {
                Success = false;
                return Success;
            }
            return Success;
        }


        //==========================================
        // Load Portfolio
        //==========================================
        public static async Task<bool> LoadPortfolio()
        {
            string fPath = AppGlobals.PortfolioFile;
            bool Success = false;

         //   await AppGlobals.ShowMessage("Saving Portfolio", fPath);

            if (System.IO.File.Exists(fPath))
            {
                AppGlobals.MyPortfolio = System.IO.File.ReadAllLines(fPath).ToList();

                if (AppGlobals.MyPortfolio.Count > 0)
                    Success = true;
            }
            return Success;
        }

        //==========================================
        // Load Trading History
        //==========================================         

        public static List<AppGlobals.TransactionItem> LoadTradingHistory(string company)
        {
                      
            var history = new List<AppGlobals.TransactionItem>();

            string fPath = Path.Combine(AppGlobals.TradingHistoryPath, company + ".csv");

            if (!File.Exists(fPath))
                return history; // Return an empty list instead of null
            try
            {
                foreach (string line in File.ReadAllLines(fPath))
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    string[] sp = line.Split(',');

                    history.Add(new AppGlobals.TransactionItem
                    {
                        Name = (sp[0]),
                        Holdings = int.Parse(sp[1]),
                        BuyPrice = decimal.Parse(sp[2]),
                        TransDate = DateTime.Parse(sp[3], CultureInfo.InvariantCulture),

                    });
                }
            }
            catch (Exception ex)
            {
                _ = AppGlobals.ShowMessage("Load Company Data", ex.Message);
               
            }
            return history;
        }

        public  static decimal LoadCompanyData(string Company, int Startindex)
        {
            decimal Open = 0;
            decimal High = 0;
            decimal Low = 0;
            decimal Close = 0;
            decimal Volume = 0;
            string fPath = "";

            // Placeholder for the return value
            List<decimal> lst_Closing = new ();

            // Ensure Startindex is set to 30 if 0
            if (Startindex == 0)
                Startindex = 30;

            
            if (Company.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
               fPath= Path.Combine(AppGlobals.DataPath, Company);                
            }
            else
            {                
                fPath = Path.Combine(AppGlobals.DataPath, Company + ".csv");
            }

            // Check if file exists
            bool fileExists = File.Exists(fPath);
            if (fileExists)
        
            AppGlobals.PeriodHigh = 0;
            AppGlobals.PeriodLow = decimal.MaxValue;

            try
            {
                if (!fileExists)
                {               
                    return 0;
                }
                else
                {
                    var lastX = File.ReadLines(fPath)
                        .Reverse()
                        .Take(Startindex)
                        .Reverse()
                        .ToList();

                    foreach (var dLine in lastX)
                    {
                        if (dLine.StartsWith("Date") || string.IsNullOrWhiteSpace(dLine))
                            continue;

                        var parts = dLine.Split(',');
                        if (parts.Length < 6)
                            continue;

                        string dDate = parts[0];
                        Open = decimal.Parse(parts[1], CultureInfo.InvariantCulture);
                        High = decimal.Parse(parts[2], CultureInfo.InvariantCulture);
                        Low = decimal.Parse(parts[3], CultureInfo.InvariantCulture);
                        Close = decimal.Parse(parts[4], CultureInfo.InvariantCulture);
                        Volume = decimal.Parse(parts[5], CultureInfo.InvariantCulture);

                        var Dt = dDate.Split('-');
                        if (Dt.Length >= 3)
                        {
                            string FmDate = Dt[2] + "-" + Dt[1] + "-" + Dt[0];
                            // Add to lists
                            {
                                
                               AppGlobals.lst_Opening.Add(Open);
                                AppGlobals.lst_high.Add(High);
                                AppGlobals.lst_low.Add(Low);
                                AppGlobals.lst_Closing.Add(Close);
                                AppGlobals.lst_volume.Add(Volume);
                                string PxD = FmDate + ",  Close  " + Close; // Price by Date list
                                AppGlobals.lst_PriceByDate.Add(PxD);
                            }
                        }
                        if (AppGlobals.PeriodHigh < High)
                        {
                            AppGlobals.PeriodHigh = High;
                            AppGlobals.Highdate = DateTime.ParseExact(dDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                        }
                        if (Low < AppGlobals.PeriodLow)
                        {
                            AppGlobals.PeriodLow = Low;
                            AppGlobals.Lowdate = DateTime.ParseExact(dDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _ = AppGlobals.ShowMessage("Load Company Data", ex.Message);
                return 0;
            }

            fPath = Path.Combine(AppGlobals.TradingHistoryPath, "TradingLog", "TradingLog.csv");

            // // Return the last closing price as decimal, or 0 if none
            if (AppGlobals.lst_Closing != null && AppGlobals.lst_Closing.Count > 0)
                 return AppGlobals.lst_Closing.Last();
 
            else
               return 0m;
        }

        public static AppGlobals.TransactionItem? GetTradeInfo(string company)
        {        
                  
            string fPath = Path.Combine(
                AppGlobals.TradingHistoryPath,
                company + ".csv");

            if (!File.Exists(fPath))
                return null;

            string[] lines = File.ReadAllLines(fPath);

            // Find the last non-empty line.
            string? lastLine = lines.LastOrDefault(line => !string.IsNullOrWhiteSpace(line));

            if (string.IsNullOrWhiteSpace(lastLine))
                return null;

            string[] fields = lastLine.Split(',');

            if (fields.Length < 4)
                return null;

            AppGlobals.TransactionItem transaction = new()
            {
                Name = fields[0],
                Holdings = int.Parse(fields[1]),
                BuyPrice = decimal.Parse(fields[2]),
                TransDate = DateTime.Parse(fields[3])
            };

            return transaction;
        }

        public static DateTime LastDataUpdate()
        {
            string fPath = AppGlobals.LastPriceUpdate;
            EnsureFolderExists(fPath);

            if (!File.Exists(fPath))
                return DateTime.MinValue;

            if (DateTime.TryParse(
                    File.ReadAllText(fPath),
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime lastUpdate))
            {
                return lastUpdate;
            }

            return DateTime.MinValue;
        }

        public static void SaveConfig()
        {
            string fPath = AppGlobals.ConfigFile;                 // use file path, not folder
            FileManager.EnsureFolderExists(fPath);                // ensure folder exists
            DateTime LastUpdate = DateTime.Today;
            string APIProvider = AppGlobals.ConfigurationManager.APIProvider;;
            string APIKey = AppGlobals.APIKey;
            int MaxHourlyRequests = AppGlobals.ConfigurationManager.MaxHourlyRequests;
            int MaxDailyRequests = AppGlobals.ConfigurationManager.MaxDailyRequests;
            int RequestCount = AppGlobals.ConfigurationManager.RequestCount;

            if (APIProvider == "" || APIProvider ==null)
             {
                APIProvider = "Tiingo";
                APIKey = "1e22624fd218a84bb88c3d777a08c7aa225190ad";
            }

            string ConfigString = $"{APIProvider},{APIKey},{MaxHourlyRequests},{MaxDailyRequests},{RequestCount},{LastUpdate}";
            System.IO.File.WriteAllText(fPath, ConfigString);
        }

        public static bool SavePortfolio()
        {
            string fPath = AppGlobals.PortfolioFile;
            EnsureFolderExists(fPath);
            try
            {
                System.IO.File.WriteAllLines(fPath, AppGlobals.MyPortfolio);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static bool SaveBalances()
        {
            string fPath = "";
            bool Success = false;

            try
            {
                 fPath = AppGlobals.PortfolioPath + "APIData.csv";
                EnsureFolderExists(fPath);

                {                    
                    //==========================================
                    // Save BankBalance
                    //==========================================
                    fPath = AppGlobals.BankBalanceFile;
                    EnsureFolderExists(fPath);
                    System.IO.File.WriteAllText(fPath, AppGlobals.BankBalance.ToString(CultureInfo.InvariantCulture));

                }
            }
            catch (Exception)
            {
                Success = false;
                return Success;
            }
            return Success;
        }

        //==========================================
        // Save Last Data Download date
        //==========================================
        public static  void SaveLastPriceUpdate()
        {
            DateTime LastUpdate = DateTime.Today;
            string fPath = AppGlobals.LastPriceUpdate;
            EnsureFolderExists(fPath);
            System.IO.File.WriteAllText(fPath, LastUpdate.ToString(CultureInfo.InvariantCulture));
        }

        public static void SaveLogFile(string data)
        {
            string fPath = Path.Combine(AppGlobals.TradingHistoryPath, "TradingLog", "TradingLog.csv");

            FileManager.EnsureFolderExists(fPath);

            if (File.Exists(fPath))
            {
                string text = File.ReadAllText(fPath);

                if (text.Length > 0 && !text.EndsWith(crlf))
                {
                    data = crlf + data;
                }
            }

            File.AppendAllText(fPath, data);
        }

        public static void SaveTrade(AppGlobals.TransactionItem Transaction)
           
        {
            string company = Transaction.Name;


            string fPath = Path.Combine(AppGlobals.TradingHistoryPath, company + ".csv");
            EnsureFolderExists(fPath);

            // If file exists, ensure it ends with newline before appending
            if (File.Exists(fPath))
            {
               
                string text = File.ReadAllText(fPath);
                if (!string.IsNullOrEmpty(text) && !text.EndsWith(crlf))
                    File.AppendAllText(fPath, Transaction + crlf);
            }

            // Append data (creates file if missing)
            File.AppendAllText(fPath, Transaction + crlf);

        }

        public static void SaveTradingHistory(string Company, string data)
        {
            string fPath = Path.Combine(AppGlobals.TradingHistoryPath, Company + ".csv");
            EnsureFolderExists(fPath);

            // If file exists, ensure it ends with newline before appending
            if (File.Exists(fPath))
            {
                string text = File.ReadAllText(fPath);
                if (!string.IsNullOrEmpty(text) && !text.EndsWith(crlf))
                    File.AppendAllText(fPath, crlf);
            }

            // Append data (creates file if missing)
            File.AppendAllText(fPath, data + crlf);
        }

        public static void SaveCompany(AppGlobals.CompanyItem company)
        {
            //string fileName = Path.Combine(AppGlobals.DataPath, "CompanyData.csv");
            string fileName = (AppGlobals.CompaniesFile);
            

            // Create the file with a header if it doesn't exist.
            if (!File.Exists(fileName))
            {
                File.WriteAllText(fileName, "Name,Symbol,Country\n");
            }

            string line = $"{company.Name},{company.Symbol},{company.Country}";

            File.AppendAllText(fileName, line + Environment.NewLine);

            // Keep the in-memory dictionary up to date.
            Dictionaries.AllCompanies[company.Name] = company.Symbol;
        }

    }
}

