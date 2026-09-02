using Microsoft.Extensions.Configuration;
using ShareTrader.Services;
using System.Collections.ObjectModel;
using static ShareTrader.Services.AppGlobals;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Controls;


namespace ShareTrader;

public partial class MainPage : ContentPage
{
    public static ObservableCollection<PortfolioItem> PortfolioItems = new();

    private ObservableCollection<CompanyItem> CompanyList =
  new ObservableCollection<CompanyItem>();

    private ObservableCollection<CompanyItem>CompaniesPopup =
        new ObservableCollection<CompanyItem>();

    public const int MaxPortfolioCompanies = 50;


    //  string fPath;
    private bool _updatingShares;
    private bool buyingShares;
    private decimal currentPrice;
    private string currentCompany = "";
    public enum TradeMode
    {
        Buy,
        Sell
    }

       private TradeMode currentTradeMode;

    private enum CompanySelectorMode
    {
        Add,
        Remove,
        Buy,
        Sell
    }

    public enum BankMode
    {
        Deposit,
        Withdraw
    }

    private BankMode currentBankMode;

    private CompanySelectorMode selectorMode;


    public MainPage()
    {
        InitializeComponent();
     //   DeviceDisplay.MainDisplayInfoChanged += OnMainDisplayInfoChanged;
        Loaded += MainPage_Loaded;
       
    }

    private async void MainPage_Loaded(object? sender, EventArgs e)
    {       

        CreateDirectories();       
        FileManager.LoadConfigData();
        BuildApiProviderMenu();

        dgPortfolio.ItemsSource = AppGlobals.PortfolioItems;

        if (GetPortfolioCount() >= MaxPortfolioCompanies)
        {
            miAddCompany.IsEnabled = false; // gray out the Add Company menu item if the portfolio is full
        }

    
        DateTime LastUpdate = AppGlobals.ConfigurationManager.LastUpdate;
        if (LastUpdate != DateTime.Today)
        {
           DownloadPrices_Clicked(sender, e);
        }
        
        await PortfolioManager.UpdatePortfolio();
        if (AppGlobals.PortfolioItems.Count > 0)
        {
            string company = AppGlobals.PortfolioItems[0].CompanyName;
            UpdatePortfolioTotals();
            Show_Analysis(company);
            
        }
    }   

    public void UpdatePortfolioTotals()
    {
        LblBankBalance.Text =
      AppGlobals.BankBalance.ToString("C2");

        LblCapitalInvested.Text =
           AppGlobals.CapitalInvested.ToString("C2");

        LblPortfolioValue.Text =
       AppGlobals.PortfolioValue.ToString("C2");

        decimal gain =
         AppGlobals.PortfolioValue -
          AppGlobals.CapitalInvested;

        LblGainLoss.Text = gain.ToString("C2");

        LblGainLoss.TextColor =
        gain >= 0 ? Colors.Green : Colors.Red;
    }

    private void ShowTradePopup(string company, decimal price, bool buy)
    {
        buyingShares = buy;
        if (buyingShares)
            currentTradeMode = TradeMode.Buy;
        else
            currentTradeMode = TradeMode.Sell;


        currentCompany = company;
        currentPrice = price;

        LblTradeTitle.Text = buyingShares ? "Buy Shares" : "Sell Shares";

        LblCompany.Text = company;
        LblPrice.Text = price.ToString("C");

        TxtShares.Text = "";

        LblTradeValue.Text = "$0.00";

        LblPopupBankBalance.Text =
           AppGlobals.BankBalance.ToString("C");

        BuySellPopup.IsVisible = true;

        TxtShares.Focus();
    }


    private async void CompanyAction_Clicked(object? sender, EventArgs e)
    {
        CompanyItem? company = gridCompanies.SelectedRow as AppGlobals.CompanyItem;


        if (company == null)
        {
            await DisplayAlert("ShareTrader",
                               "Please select a company.",
                               "OK"); 
            return;
        }    

    string companyName = company.Name;
    string companySymbol = company.Symbol;

        decimal SharePrice = FileManager.LoadCompanyData(companyName, 1);

        switch (selectorMode)
        {
            case CompanySelectorMode.Add:
                miAddCompany.IsEnabled = MyPortfolio.Count < MaxPortfolioCompanies;
                await PortfolioManager.AddSelectedCompany(companyName, companySymbol);
                // Tell Syncfusion to refresh.
                dgPortfolio.ItemsSource = null;
               dgPortfolio.ItemsSource = AppGlobals.PortfolioItems;
                // Force SfDataGrid to redraw.
               dgPortfolio.View?.Refresh();
                dgPortfolio.InvalidateMeasure();              
               
                break;
            case CompanySelectorMode.Remove:
               await  PortfolioManager.RemovePortfolioItem(companyName, "0");
                btnAddNewCompany.IsVisible = false;
                break;
            case CompanySelectorMode.Buy:
                ShowTradePopup(company.Name, SharePrice, true); //true if Buying, false if selling
                break;
            case CompanySelectorMode.Sell:
                ShowTradePopup(company.Name, SharePrice, false);  //true if Buying, false if selling
                break;
        }

        CompanySelector.IsVisible = false;
       
    }

        private async void AddCompany_Clicked(object? sender, EventArgs e)
    {
        string fPath = CompaniesFile;
        selectorMode = CompanySelectorMode.Add;

        BtnCompanyAction.Text = "Add Company";
        LblCompanySelectorTitle.Text = "Add Company";
        TxtSearch.Text = "";
        TxtSearch.Focus();
        btnAddNewCompany.IsVisible = MyPortfolio.Count < MaxPortfolioCompanies;
      await  LoadCompanySelector(fPath);
        CompanySelector.IsVisible = true;
    }

    private async void btnAddNewCompany_Clicked(object? sender, EventArgs e)
    {
        var popup = new AddCompanyPopup();

        // When the popup saves a company, refresh the company grid.
        popup.CompanyAdded += async () =>
        {
            await RefreshCompanyGrid();
        };

        await Navigation.PushModalAsync(popup);
    }

    private  async void RemoveCompany_Clicked(object? sender, EventArgs e)
    {
        string fPath = PortfolioFile;
        selectorMode = CompanySelectorMode.Remove;
        LblCompanySelectorTitle.Text = "Remove Company";
        BtnCompanyAction.Text = "Remove Company";
        TxtSearch.Text = "";
        TxtSearch.Focus();
        btnAddNewCompany.IsVisible = false;

       await LoadCompanySelector(fPath);
        CompanySelector.IsVisible = true;

    }

   void TxtSearch_TextChanged(object? sender, EventArgs e)
    {
    }

    async void SavePortfolio_Clicked(object? sender, EventArgs e)
    {
        FileManager.SaveConfig();
        FileManager.SaveBalances();
        FileManager.SaveLastPriceUpdate();
        FileManager.SaveLogFile("Portfolio saved at " + DateTime.Now.ToString());

        await DisplayAlert("Portfolio", "Portfolio Saved.", "OK");
    }

   private async void BuyShares_Clicked(object? sender, EventArgs e)
    {
        string fPath = AppGlobals.PortfolioFile;
        selectorMode = CompanySelectorMode.Buy;
        currentTradeMode = TradeMode.Buy;
        LblCompanySelectorTitle.Text = "Buy Shares";
        BtnCompanyAction.Text = "Buy Shares";
        TxtSearch.Text = "";
        TxtSearch.Focus();
      await  LoadCompanySelector(fPath);
        CompanySelector.IsVisible = true;
    }

    private async void SellShares_Clicked(object? sender, EventArgs e)
    {
        string fPath = AppGlobals.PortfolioFile;
        selectorMode = CompanySelectorMode.Sell;
        currentTradeMode = TradeMode.Sell;
        LblCompanySelectorTitle.Text = "Sell Shares";
        BtnCompanyAction.Text = "Sell Shares";
        TxtSearch.Text = "";
        TxtSearch.Focus();
     await   LoadCompanySelector(fPath);
        CompanySelector.IsVisible = true;
    }
    void Deposit_Clicked(object? sender, EventArgs e)
    {
        currentBankMode = BankMode.Deposit;

        LblBankTitle.Text = "Deposit Funds";
        LblCurrentBalance.Text = AppGlobals.BankBalance.ToString("C");
        LblNewBalance.Text = AppGlobals.BankBalance.ToString("C");
        TxtBankAmount.Text = "";
        BankPopup.IsVisible = true;
        TxtBankAmount.Focus();
    }

    private void BuildApiProviderMenu()
    {
        apiProviderMenu.Clear();

        foreach (string provider in AppGlobals.ApiProviders)
        {
            var item = new MenuFlyoutItem
            {
                Text = provider == AppGlobals.ConfigurationManager.APIProvider
                ? $"✓ {provider}"
                : provider
            };

            item.Clicked += ApiProviderItem_Clicked;

            apiProviderMenu.Add(item);
        }
    }

    private async void ApiProviderItem_Clicked(object? sender, EventArgs e)
    {
        if (sender is not MenuFlyoutItem item)
            return;

        // Remove the ✓ if it was clicked.
        string provider = item.Text.Replace("✓ ", "");


        switch (provider)
        {
            case "Tiingo":
                AppGlobals.ConfigurationManager.APIProvider = "Tiingo";
                AppGlobals.APIKey = "1e22624fd218a84bb88c3d777a08c7aa225190ad";
                break;

            case "AlphaVantage":
                AppGlobals.ConfigurationManager.APIProvider="AlphaVantage";
                AppGlobals.APIKey = "JNH36WFVGKTM5DLH";
                break;

            case "TwelveData":
                AppGlobals.ConfigurationManager.APIProvider = "TwelveData";
                AppGlobals.APIKey = "63cafd3b3e4644d4beb5ad5e3656ca05";
                break;

            case "MarketStack":
                AppGlobals.ConfigurationManager.APIProvider = "MarketStack";
                AppGlobals.APIKey = "01bec4608e83f98fb6f4294c8b6a9db4";
                break;

            case "Auto":
                AppGlobals.ConfigurationManager.APIProvider = "Auto";
                AppGlobals.APIKey = "";
                break;

            default:
                return;     // Unknown provider.
        }

        // Rebuild the menu so the tick moves.
        BuildApiProviderMenu();


        FileManager.SaveConfig();

        await AppGlobals.ShowMessage(
            "API Provider",
            $"Now using {AppGlobals.ConfigurationManager.APIProvider}");
    }


    void Withdraw_Clicked(object? sender, EventArgs e)
    {
        currentBankMode = BankMode.Withdraw;

        LblBankTitle.Text = "Withdraw Funds";
        LblCurrentBalance.Text = AppGlobals.BankBalance.ToString("C");
        LblNewBalance.Text = AppGlobals.BankBalance.ToString("C");
        TxtBankAmount.Text = "";        
        BankPopup.IsVisible = true;
        TxtBankAmount.Focus();
    }

    void TxtBankAmount_TextChanged(object? sender, TextChangedEventArgs e)
    {
        if (!decimal.TryParse(e.NewTextValue, out decimal amount))
        {
            LblNewBalance.Text = AppGlobals.BankBalance.ToString("C");
            return;
        }

        decimal newBalance = currentBankMode == BankMode.Deposit
            ? AppGlobals.BankBalance + amount
            : AppGlobals.BankBalance - amount;

        LblNewBalance.Text = newBalance.ToString("C");
    }
    async void BtnBankOK_Clicked(object? sender, EventArgs e)
    {
        if (!decimal.TryParse(TxtBankAmount.Text, out decimal amount))
        {
            await DisplayAlert("Error", "Please enter a valid amount.", "OK");
            TxtBankAmount.Focus();
            return;
        }

        if (currentBankMode == BankMode.Deposit)
            AppGlobals.BankBalance += amount;
        else
            AppGlobals.BankBalance -= amount;

        FileManager.SaveBalances();
        UpdatePortfolioTotals();

        BankPopup.IsVisible = false;
    }

    void BtnBankCancel_Clicked(object? sender, EventArgs e)
    {
        BankPopup.IsVisible = false;
        return;
    } 

    async void DownloadPrices_Clicked(object? sender, EventArgs e)
    {
        await Services.DownloadService.UpdateSharePrices(busyIndicator, BusyOverlay);
   }


    async void Settings_Clicked(object? sender, EventArgs e)
        => await DisplayAlert("Tools", "Settings not available in this version.", "OK");

    async void Register_Clicked(object? sender, EventArgs e)
    {
        string Message = "Registration is not required in this version." + Environment.NewLine +
            "However if you use and enjoy the app, please consider supporting us." + Environment.NewLine +
            "Visit our website for more information." + Environment.NewLine + "camsoftAU@gmail.com"+ Environment.NewLine +
            "Thank you for your support."; 
             await DisplayAlert("Register", Message,"OK");
         }

     void Reset_Clicked(object? sender, EventArgs e)
    {
        Reset.ResetAlldData();
    }  

    async void About_Clicked(object sender, EventArgs e)
    => await DisplayAlert("Share Trading Simulation", "Version 1.0.0", "OK");


    private void ShowManual(object sender, EventArgs e)
    {
        AnalysisView.IsVisible = false;
        TextView.IsVisible = true;

        LblAnalysisTitle.Text = "Manual";

        Information.Text = TextFiles.Manual();
    }

    void Strategies_Clicked(object sender, EventArgs e)
    {
        AnalysisView.IsVisible = false;
        TextView.IsVisible = true;

        LblAnalysisTitle.Text = "Trading Strategy";

        Information.Text = TextFiles.TradingStrategies();
    }

    async void LogFile_Clicked(object sender, EventArgs e)
       => await DisplayAlert("Help", "Log File Clicked.", "OK");


    private void Show_Analysis(string company)
    {
       dgPortfolio.ItemsSource = AppGlobals.PortfolioItems;

        if (AppGlobals.PortfolioItems.Count == 0)
            return;
      string Recomendation = AnalysisPanel.GetRecommendation(company);


        TextView.IsVisible = false;
        AnalysisView.IsVisible = true;

        LblAnalysisTitle.Text =
             "Analyzing Trends over the past 100 days for " + company;      
      
        lstPriceByDate.ItemsSource = ChartManager.FillPxDateList(company);

        var summary = AnalysisPanel.GetPriceSummary();

        LblAnalysis2Row1.Text = summary.Low;
        LblAnalysis2Row2.Text = summary.High;
        TxtAnalysis1.Text = AnalysisPanel.GetRecommendation(company);
        AnalysisPanel.DisplayAnalysis(
            BollingerChart,
            AdxChart,
            VolumeChart,
            company);

        var purchase = AnalysisPanel.GetPurchaseSummary(company);

        LblAnalysis2Row3.Text = purchase.Row3;
        LblAnalysis2Row4.Text = purchase.Row4;

    }

   private void dgPortfolio_SelectionChanged(
    object sender,
    Syncfusion.Maui.DataGrid.DataGridSelectionChangedEventArgs e)
{
    if (e.AddedRows == null || e.AddedRows.Count == 0)
        return;

    if (e.AddedRows[0] is not PortfolioItem item)
        return;

    string company = item.CompanyName;

    TextView.IsVisible = false;
    AnalysisView.IsVisible = true;

        Show_Analysis(company);      
}

    private void InformationPanel_LostFocuc(object sender, EventArgs e)
    {
        InformationPanel.IsVisible = false;
    }

    private void TxtShares_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (_updatingShares)
            return;

        decimal tradeValue =
            ShareTrading.CalculateTradeValue(e.NewTextValue, currentPrice);

        if (tradeValue == -1m)
        {
            _updatingShares = true;
            ((Entry)sender).Text = e.OldTextValue;
            _updatingShares = false;
            return;
        }

        LblTradeValue.Text = tradeValue.ToString("C2");
    }

    private async void BtnTradeOK_Clicked(object sender, EventArgs e)
    {
        if (!int.TryParse(TxtShares.Text, out int shares))
        {
            await DisplayAlert("Error", "Please enter the number of shares.", "OK");
            return;
        }

        // Use the values here        

        BuySellPopup.IsVisible = false;
        decimal tradeValue = shares * currentPrice;

        if (currentTradeMode == TradeMode.Buy)
        {
            // Buy the shares
            await ShareTrading.BuyShares(currentCompany, shares, currentPrice);

        }
        else
        {
            // Sell the shares
            await ShareTrading.SellShares(currentCompany, shares, currentPrice);
        }

      await PortfolioManager.UpdatePortfolio();
        UpdatePortfolioTotals();
        BuySellPopup.IsVisible = false;
    }

     void BtnTradeCancel_Clicked(object sender, EventArgs e)
    {
        // Hide the Buy/Sell popup when cancel is clicked
        BuySellPopup.IsVisible = false;
    }

    private void CancelCompanySelection_Clicked(object sender, EventArgs e)
    {
        CompanySelector.IsVisible = false;
    }

    public async Task LoadCompanySelector(string fPath)
    {
        CompaniesPopup.Clear();


        if (!File.Exists(fPath))
        {
            // First run - populate from built-in dictionary.
            foreach (var company in Dictionaries.AllCompanies.OrderBy(c => c.Key))
            {
                var item = new CompanyItem
                {
                    Name = company.Key,
                    Symbol = company.Value
                };

                CompaniesPopup.Add(item);

                // Save the initial file.
                FileManager.SaveCompany(item);
            }
        }
        else
        {            
                // Load from file
                foreach (string line in File.ReadAllLines(fPath))
                
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    string[] parts = line.Split(',');
                    if (parts.Length >= 2)
                    {
                        CompaniesPopup.Add(new CompanyItem
                        {
                            Name = parts[0].Trim(),
                            Symbol = parts[1].Trim()
                        });

                    SortCompaniesPopup();

                }
          }
     }

            gridCompanies.ItemsSource = null;
            gridCompanies.ItemsSource = CompaniesPopup;

 }

    private void SortCompaniesPopup()
    {
        var ordered = CompaniesPopup
            .OrderBy(c => c.Name, StringComparer.CurrentCultureIgnoreCase)
            .ToList();

        CompaniesPopup.Clear();
        foreach (var c in ordered)
            CompaniesPopup.Add(c);

        // If a grid is bound to CompaniesPopup, refresh it (Syncfusion example)
        gridCompanies?.View?.Refresh();
    }

    public static int GetPortfolioCount()
    {
        if (!File.Exists(AppGlobals.PortfolioFile))
            return 0;

        return File.ReadLines(AppGlobals.PortfolioFile)
                   .Count(line => !string.IsNullOrWhiteSpace(line));
    }

    private async Task RefreshCompanyGrid()
    {
      await  LoadCompanySelector(AppGlobals.CompaniesFile);

        gridCompanies.ItemsSource = null;
        gridCompanies.ItemsSource = CompaniesPopup;

        if (CompaniesPopup.Count > 0)
        {
            int lastRow = CompaniesPopup.Count - 1;

            gridCompanies.SelectedIndex = lastRow;
           await gridCompanies.ScrollToRowIndex(lastRow, ScrollToPosition.End);
        }
    }

    private void OnMainDisplayInfoChanged(object? sender, DisplayInfoChangedEventArgs e)
    {
        // Update when orientation or density changes
        SetPageHeightToScreen(e.DisplayInfo);
    }

    private void SetPageHeightToScreen(DisplayInfo? info = null)
    {
        var display = info ?? DeviceDisplay.MainDisplayInfo;
        // DisplayInfo.Height is in physical pixels; divide by Density to get device-independent units (DIP)
        double screenHeightDip = display.Height / display.Density;

        // Apply to root layout (preferred) or the page itself
    // RootGrid.HeightRequest = screenHeightDip;
        // Optionally: this.HeightRequest = screenHeightDip;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        DeviceDisplay.MainDisplayInfoChanged -= OnMainDisplayInfoChanged;
    }
}
