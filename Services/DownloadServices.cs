using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ShareTrader.Services
{

  
  public class DownloadServices
                  
    {
       static bool DownloadSucceeded = false;

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

                DownloadSucceeded = await GetData(name, symbol);
            }

            if (DownloadSucceeded)
            {
                FileManager.SaveLastPriceUpdate();
                await PortfolioManager.UpdatePortfolio();               
            }         

            busyIndicator.IsRunning = false;
            busyIndicator.IsVisible = false;
            BusyOverlay.IsVisible = false;
        }

        public static async Task<bool> GetData(string CompanyName, string symbol)
        {
           
            string dP = AppGlobals.CurrentApiProvider;
            String APIKey = AppGlobals.APIKey;
            if (string.IsNullOrWhiteSpace(dP) || dP is "Auto")
            {
                dP = "Tiingo";
                APIKey = "1e22624fd218a84bb88c3d777a08c7aa225190ad";
            }
            string tData = "";
            DateTime startDate = DateTime.Today.AddDays(-365);
            DateTime endDate = DateTime.Today;

            string fPath = Path.Combine(AppGlobals.DataPath, CompanyName + ".csv");

            string tempPath = Path.Combine(AppGlobals.DataPath, CompanyName + ".tmp");

            string url = "";
            int pause = 0;
            string country ="ax";
            string cKey = ConvertSymbol(symbol, dP);

            DownloadSucceeded = false;

            try
            {
                switch (dP)
                {
                    case "Tiingo":                        

                        APIKey = "1e22624fd218a84bb88c3d777a08c7aa225190ad";
                        pause = 120;

                        url = $"https://api.tiingo.com/tiingo/daily/{cKey}/prices?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}&token={APIKey}";
                        break;

                    case "AlphaVantage":
                        APIKey = "JNH36WFVGKTM5DLH";
                        pause = 12000;
                        url = $"https://www.alphavantage.co/query?function=TIME_SERIES_DAILY&symbol={cKey}&outputsize=compact&apikey={APIKey}";
                        break;

                    case "TwelveData":
                        pause = 1200;
                        APIKey = "63cafd3b3e4644d4beb5ad5e3656ca05";
                        url = $"https://api.twelvedata.com/time_series?symbol={cKey}&interval=1day&apikey={APIKey}&format=CSV"; break;

                    case "MarketStack":
                        APIKey = "01bec4608e83f98fb6f4294c8b6a9db4";
                        pause = 1200;

                        // Replace with your MarketStack URL.
                        url = $"https://api.marketstack.com/v1/eod?access_key={APIKey}&symbols={cKey}&date_from={startDate:yyyy-MM-dd}&date_to={endDate:yyyy-MM-dd}&limit=365"; break;

                    default:
                        return false;
                }

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
                    HttpResponseMessage response = await client.GetAsync(url);

                    tData = await response.Content.ReadAsStringAsync();

                    if (!DataIsValid(dP, tData))
                    {
                        string message = $"{dP} does not support the requested data with the free version.";
                        await AppGlobals.ShowMessage
                            ($"Error downloading data for {CompanyName} from {dP}.",
                           message);

                        return false;
                    }
                    ;

                    tData = tData.Trim();

                    if (!DataIsValid(dP, tData))
                        return false;

                    // No data returned
                    if (string.IsNullOrWhiteSpace(tData) || tData == "[]")
                        return false;

                    await Task.Delay(pause);

                    DownloadSucceeded = true;
                }
            }

            catch (Exception)
            {
                return false;
            }

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

                if ( dP == "AlphaVantage"|| dP == "MarketStack")
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
                else if (dP == "Stooq" || dP == "Tiingo" )
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
                }
            }
            catch (Exception)
            {
                // Handle or log exception as needed
            }


            return DownloadSucceeded;
        }
        public static string ConvertSymbol(string symbol, string provider)
        {
            symbol = symbol.Trim();
            int n;
            string symbl="";
            string country ="";

            string[] parts = symbol.Split('.');

            string code = parts[0];
            string market = parts.Length > 1 ? parts[1].ToUpper() : "";

            switch (provider)
            {
                case "Tiingo":
                    return market switch
                    {
                        "AX" => $"asx:{code.ToLower()}",
                        "NZ" => $"nzx:{code.ToLower()}",
                        "UK" => $"lse:{code.ToLower()}",
                        _ => code // Default case for other markets
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

        private string GetCountry(string symbol)
        {
            string[] parts = symbol.Split('.');

            if (parts.Length == 1)
                return "USA";

            return parts[1].ToUpper();
        }

    }
}

