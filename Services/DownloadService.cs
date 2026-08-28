using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ShareTrader.Services
{
    internal class DownloadService
    {
        static bool DownloadSucceeded = false;
        static string provider = AppGlobals.ConfigurationManager.APIProvider;
        static string apiKey = AppGlobals.APIKey;


        public static async Task UpdateSharePrices(
            ActivityIndicator busyIndicator,
             Grid BusyOverlay)
        {
            string fPath = AppGlobals.PortfolioFile;
            DownloadSucceeded = false;


            if (!File.Exists(fPath))
                return;

            busyIndicator.IsVisible = true;
            busyIndicator.IsRunning = true;
            BusyOverlay.IsVisible = true;

            string[] lines = File.ReadAllLines(fPath);

            foreach (string line in lines)
            {
                string[] sd = line.Split(',');

                string name = sd[0];
                string symbol = sd[1];

              
                DownloadSucceeded = await DownloadData(
                    symbol,
                    name);
            }

            busyIndicator.IsRunning = false;
            busyIndicator.IsVisible = false;
            BusyOverlay.IsVisible = false;
        }

        public static async Task<bool> DownloadFromProvider(
    string provider,
    string companyName,
    string symbol) 

        {
            string apiSymbol = ConvertSymbol(symbol, provider);

            await AppGlobals.ShowLoading(
                "Downloading",
                $"Downloading {companyName}\nUsing {provider}...");

            bool success = false;

            try
            {
            //https://api.tiingo.com/tiingo/daily/ald/prices?startDate=2025-08-01&endDate=2025-08-31&token=YOURTOKEN

                string url = BuildApiUrl(provider, apiSymbol);

                //url = $"https://api.tiingo.com/tiingo/daily/tls?token=1e22624fd218a84bb88c3d777a08c7aa225190ad";

               

                using HttpClient client = new();

                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");

                HttpResponseMessage response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                  return false;

                string data = await response.Content.ReadAsStringAsync();

                if (!DataIsValid(provider, data))
                return false;

                success = await SaveData(provider, companyName, data);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"DownloadFromProvider ({provider}) : {ex.Message}");
            }
            finally
            {
                await AppGlobals.HideLoading();
            }

            return success;
        }

        public async static Task<bool> SaveData(String provider,string CompanyName,string tData)
        {
            string fPath = Path.Combine(AppGlobals.DataPath, CompanyName + ".csv");

            string tempPath = Path.Combine(AppGlobals.DataPath, CompanyName + ".tmp");

            //----------------------------------------------------
            // JSON parsing starts here 
            //----------------------------------------------------


            StringBuilder sb = new StringBuilder();

            try
            {
                if (string.IsNullOrWhiteSpace(tData) || tData == "[]")
                {
                    return false;
                }

                if (tData.Contains("Error"))
                {

                    return false;
                }

                sb.Clear();

                if (provider == "AlphaVantage" || provider == "MarketStack")
                {
                    using (JsonDocument doc = JsonDocument.Parse(tData))
                    {
                        JsonElement series = doc.RootElement.GetProperty("Time Series (Daily)");

                        foreach (JsonProperty day in series.EnumerateObject().Reverse())
                        {
                            DateTime d = DateTime.Parse(day.Name);

                            string o = day.Value.GetProperty("1. open").GetString();
                            string h = day.Value.GetProperty("2. high").GetString();
                            string l = day.Value.GetProperty("3. low").GetString();
                            string c = day.Value.GetProperty("4. close").GetString();
                            string v = day.Value.GetProperty("5. volume").GetString();

                            sb.AppendLine($"{d:yyyy-MM-dd},{o},{h},{l},{c},{v}");
                        }
                    }

                    if (File.Exists(tempPath))
                        File.Delete(tempPath);
                }
                else if (provider == "TwelveData" || provider == "Tiingo")
                {
                    using (JsonDocument doc = JsonDocument.Parse(tData))
                    {
                        foreach (JsonElement item in doc.RootElement.EnumerateArray())
                        {
                            DateTime d = DateTime.Parse(item.GetProperty("date").GetString());

                            decimal o = item.GetProperty("open").GetDecimal();
                            decimal h = item.GetProperty("high").GetDecimal();
                            decimal l = item.GetProperty("low").GetDecimal();
                            decimal c = item.GetProperty("close").GetDecimal();
                            long v = item.GetProperty("volume").GetInt64();

                            sb.AppendLine($"{d:yyyy-MM-dd},{o},{h},{l},{c},{v}");
                        }
                    }
                }

                if (sb.Length > 20)
                {
                    if (!File.Exists(tempPath))
                        File.Create(tempPath).Dispose();

                    using (StreamWriter sw = new StreamWriter(tempPath, true))
                    {
                        sw.Write(sb.ToString());
                    }
                    if (File.Exists(fPath))
                    {
                        File.Delete(fPath);
                    }

                    File.Copy(tempPath, fPath);
                    return true;
                }
            }
            catch (Exception)
            {
                // Handle or log exception as needed
                return false;
            }
            
            return true;
        }
       
        public static string ConvertSymbol(string symbol, string provider)
        {
            symbol = symbol.Trim();
            int n;
            string symbl = "";
            string country = "";

            string[] parts = symbol.Split('.');

            string code = parts[0];
            string market = parts.Length > 1 ? parts[1].ToUpper() : "";

            switch (provider)
            {
                case "Tiingo":
                    return market switch
                    {
                        "AX" => code.ToLower(),
                        "NZ" => code.ToLower(),
                        "UK" => code.ToLower(),
                        "US" => code.ToLower(),
                        _ => code.ToLower()
                    };
                case "AlphaVantage":
                    return symbol;

                case "TwelveData":
                    return market switch
                    {
                        "AX" => $"{code}:ASX",
                        "NZ" => $"{code}:NZX",
                        "UK" => $"{code}:LSE",
                        _ => code// Default case for other markets
                    };

                case "MarketStack":
                    return symbol;

                default:
                    return symbol;
            }
        }

        public static bool DataIsValid(string provider, string data)
        {
            switch (provider)
            {
                case "AlphaVantage":
                    return !(data.Contains("\"Error Message\"") ||
                             data.Contains("\"Note\""));

                case "Tiingo":
                    return data.Trim() != "[]";

                case "MarketStack":
                    return !data.Contains("\"error\"") &&
                           !data.Contains("\"data\":[]");

                case "TwelveData":
                    return !string.IsNullOrWhiteSpace(data) &&
                           !data.Contains("No data") &&
                           !data.Contains("404");

                default:
                    return !string.IsNullOrWhiteSpace(data);
            }
        }

        // Helper method to build the API URL based on the provider and symbol

        public static string BuildApiUrl(string provider, string symbol)
        {
            string url = "";

            switch (provider)
            {
                case "Tiingo":
                    apiKey = "1e22624fd218a84bb88c3d777a08c7aa225190ad";
                    DateTime startDate = DateTime.Today.AddDays(-365);
                    DateTime endDate = DateTime.Today;
                    url = $"https://api.tiingo.com/tiingo/daily/{symbol}/prices?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}&token={apiKey}";
                    break;
                case "AlphaVantage":
                    apiKey = "JNH36WFVGKTM5DLH";
                    url = $"https://www.alphavantage.co/query?function=TIME_SERIES_DAILY&symbol={symbol}&apikey={apiKey}";
                    break;
                case "TwelveData":
                    apiKey = "63cafd3b3e4644d4beb5ad5e3656ca05";
                    url = $"https://api.twelvedata.com/time_series?symbol={symbol}&interval=1day&format=CSV&apikey=apiKey";
                    break;
                case "MarketStack":
                    apiKey = "01bec4608e83f98fb6f4294c8b6a9db4";
                    url = $"https://api.marketstack.com/v1/eod?symbols={symbol}&access_key={apiKey}";
                    break;
            }
            return url;
        }

        public static async Task<bool> DownloadData(string symbol, string companyName)
        {
            // Manual provider selected?

           // AppGlobals.ConfigurationManager.APIProvider = "Auto";

            if (AppGlobals.ConfigurationManager.APIProvider != "Auto")
            {
                return await DownloadFromProvider(
                    AppGlobals.ConfigurationManager.APIProvider,
                     companyName,
                    symbol)
                   ;
            }

            // ----- AUTO MODE -----

            string country = GetCountry(symbol);

            List<string> providers = country switch
            {
                "AX" => new() { "Tiingo", "MarketStack", "AlphaVantage", "TwelveData" },
                "NZ" => new() { "Tiingo", "MarketStack", "TwelveData", "AlphaVantage" },
                "UK" => new() { "Tiingo", "MarketStack", "TwelveData", "AlphaVantage" },
                _ => new() { "Tiingo", "AlphaVantage", "MarketStack", "TwelveData" }
            };

            foreach (string provider in providers)
            {
                bool ok = await DownloadFromProvider(provider,companyName,symbol);

                if (ok)
                {
                    AppGlobals.APIProvider = provider;     // Optional.
                    return true;
                }
            }

            return false;
        }
        private static string GetCountry(string symbol)
        {
            string[] parts = symbol.Split('.');

            if (parts.Length == 1)
                return "USA";

            return parts[1].ToUpper();
        }

    }
}