using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using static Java.Util.Jar.Attributes;

namespace ShareTrader.Services
{
    public static class AppGlobals
    {

        public static readonly string RootPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ShareTrader");
        public static readonly string ConfigPath = Path.Combine(RootPath, "Config");
        public static readonly string DataPath = Path.Combine(RootPath, "Data");
        public static readonly string PortfolioPath = Path.Combine(RootPath, "Portfolio");
        public static readonly string TradingHistoryPath = Path.Combine(PortfolioPath, "TradingHistory");
        public static readonly string LogPath = Path.Combine(PortfolioPath, "TradingHistory","TradingLog");
        public static readonly string CompaniesPath = Path.Combine(DataPath, "Companies");


        public static readonly string ConfigFile = Path.Combine(ConfigPath, "APIData.csv");
        public static readonly string CompaniesFile = Path.Combine(CompaniesPath,"Companies.csv");
        public static readonly string PortfolioFile = Path.Combine(PortfolioPath, "MyPortfolio.csv");
        public static readonly string BankBalanceFile = Path.Combine(PortfolioPath, "BankBalance.csv");
        public static readonly string LastPriceUpdate = Path.Combine(PortfolioPath, "LastPriceUpdate.csv");

        public static readonly string LogFile = Path.Combine(LogPath, "TradingLog.csv");


        public static bool NewDataRequested;
        public static bool ProgramRegistered;
        public static string regkey ="";
        public static bool Success;
        public static Page? LoadingPage;

        //=====API info=======
        public static string APIProvider ="";
        public static string APIKey ="";
        public static int maxHourlyRequests =50;
        public static int maxDailyRequests = 1000;
        public static int RequestCount = 0
;       public static DateTime LastUpdate = DateTime.Today;
        //====================================
       

        //====Share trading info============

        public static decimal High;
        public static decimal Low;
        public static decimal PeriodHigh;
        public static decimal PeriodLow;
        public static DateTime Highdate;
        public static DateTime Lowdate;       
       

        public static string ProviderMessage ="";    

      
        public static List<string> lst_Companydata = new List<string>();
      
        //===Share Data lists=========
        public static List<decimal> lst_Opening = new List<decimal>();
        public static List<decimal> lst_Closing = new List<decimal>();
        public static List<decimal> lst_volume = new List<decimal>();
        public static List<decimal> lst_high = new List<decimal>();
        public static List<decimal> lst_low = new List<decimal>();
        public static List<string> lst_PriceByDate = new List<string>();

        //=====================================================
        // Portfolio Data
        //=====================================================


        public static decimal PortfolioValue;
        public static string CompanyName ="";
        public static decimal BankingData = 0;
        public static decimal BankBalance = 0;
        public static int Holdings = 0;
      //  public static decimal SharePrice = 0;
        public static string Trend = "";
        public static string BuySellMessage ="";
        public static string signal ="";
        public static int Period = 30;
        public static string BuyingHistory ="";
        public static decimal CapitalInvested = 0;
        public static decimal GainsLosses = 0;
        public static string BankingHistory = "";

        public static string Version ="1.0.0";
        public static class ConfigurationManager
        {
            public static string APIProvider ="";
            public static string APIKey ="";
            public static int MaxHourlyRequests = 0;
            public static int MaxDailyRequests = 0;
            public static int RequestCount = 0;
            public static DateTime LastUpdate = DateTime.MinValue;
        }

        // Your existing globals...
        public static string CurrentApiProvider ="";

        public static List<string> MyPortfolio = new();

        public static ObservableCollection<PortfolioItem> PortfolioItems
            = new ObservableCollection<PortfolioItem>();

        public static readonly List<string> ApiProviders = new()
            {
                "Auto",
                "Tiingo",
                "AlphaVantage",
                "TwelveData",
                "MarketStack"                
            };

        public class PortfolioItem
        {
            public string CompanyName { get; set; } = "";
            public string  Trend { get; set; } = "";
            public int Shares { get; set; } = 0;
            public decimal BuyPrice { get; set; } = 0;
            public decimal TotalCost { get; set; } = 0;                     
            public decimal CurrentPrice { get; set; } = 0;
            public decimal Value { get; set; } = 0;
            public decimal Profit { get; set; } = 0;
        }


        public static void CreateDirectories()
         {
             Directory.CreateDirectory(RootPath);
             Directory.CreateDirectory(DataPath);
             Directory.CreateDirectory(PortfolioPath);
             Directory.CreateDirectory(ConfigPath);
             Directory.CreateDirectory(TradingHistoryPath);
             Directory.CreateDirectory(LogPath);
             Directory.CreateDirectory(CompaniesPath);

        }

        public class CompanyItem
        {
            public string Name { get; set; } = "";
            public string Symbol { get; set; } = "";
            public string Country { get; set; } = "";
        }
          
        public static PortfolioItem SelectedPortfolioItem { get; set; } = new PortfolioItem();

        public class TransactionItem
        {
            public string Name = "";
            public int Holdings = 0;           
            public decimal BuyPrice = 0; // The price we paid for the last transaction           
            public DateTime TransDate = DateTime.Today;            
        }

              public static async Task ShowLoading(string title, string message)
        {
            LoadingPage = new ContentPage
            {
                BackgroundColor = Color.FromArgb("#80000000"),
                Content = new VerticalStackLayout
                {
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center,
                    Spacing = 15,
                    Children =
            {
                new ActivityIndicator
                {
                    IsRunning = true,
                    WidthRequest = 40,
                    HeightRequest = 40
                },

                new Label
                {
                    Text = title,
                    FontAttributes = FontAttributes.Bold,
                    HorizontalTextAlignment = TextAlignment.Center,
                    TextColor = Colors.White
                },

                new Label
                {
                    Text = message,
                    HorizontalTextAlignment = TextAlignment.Center,
                    TextColor = Colors.White
                }
            }
                }
            };

            var page = Application.Current?.Windows.FirstOrDefault()?.Page;

            if (page != null)
            {
                await page.Navigation.PushModalAsync(LoadingPage, false);
            }
        }
        public static async Task HideLoading()
        {
            if (LoadingPage != null)
            {
                var page = Application.Current?.Windows.FirstOrDefault()?.Page;

                if (page != null)
                {
                    await page.Navigation.PopModalAsync(false);
                }

                LoadingPage = null;
            }
        }


        public static async Task ShowMessage(string title, string message)
        {
            var page = Application.Current?.Windows[0].Page;

            if (page != null)
                await page.DisplayAlert(title, message, "OK");
        }


        //====Example===
        //       bool answer = await Application.Current.MainPage.DisplayAlert(
        //               "Remove Company",
        //                "Are you sure you want to remove Tesla?",
        //                "Yes",
        //                "No");

        /// if (answer)
        //  {
        // User clicked Yes
        //  }
        //      else
        //  {
        // User clicked No
        //  }


        public static async Task<string> AskYesNoCancel(string title, string message)
        {
            var page = Application.Current?.Windows.FirstOrDefault()?.Page;

            if (page != null)
            {
                bool yes = await page.DisplayAlert(title, message, "Yes", "No");

                return yes ? "Yes" : "No";
            }

            return "Cancel";
        }

        //====Example===
        //          if (result == "Yes")
        //          {
        // Remove company
        //          }
        //          else if (result == "No")
        //          {
        // Don't remove
        //          }
        //          else
        //          {
        // Cancel pressed
        //          }


    }
}
