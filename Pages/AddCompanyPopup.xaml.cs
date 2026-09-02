namespace ShareTrader;

using ShareTrader.Services;

public partial class AddCompanyPopup : ContentPage
{

    public event Action? CompanyAdded;
    public AppGlobals.CompanyItem NewCompany { get; set; } = new AppGlobals.CompanyItem();
    private List<AppGlobals.CompanyItem> CompaniesPopup = new();

    public AddCompanyPopup()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await Task.Delay(100);
        txtName.Focus();
    }


    private async void Cancel_Clicked(object sender, EventArgs e)
     {
        await Navigation.PopModalAsync();
    }

    private async void Save_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtName.Text) ||
            string.IsNullOrWhiteSpace(txtSymbol.Text) ||
            pkCountry.SelectedIndex < 0)
        {
            await DisplayAlert(
                "Missing Information",
                "Please enter Company Name, Symbol and Country.",
                "OK");
            return;
        }

        if (pkCountry.SelectedIndex != -1)
        {
            NewCompany = new AppGlobals.CompanyItem
            {
                Name = txtName.Text?.Trim() ?? "",
                Symbol = txtSymbol.Text?.Trim().ToUpper() ?? "",
                Country = pkCountry.SelectedItem?.ToString() ?? "",
            };
        }


        FileManager.SaveCompany(NewCompany);

        await DisplayAlert(
            "Company Added",
            $"{NewCompany.Name} ({NewCompany.Symbol}) has been added.",
            "OK");

      CompanyAdded?.Invoke(); // Tell MainPage to refresh.

        await Navigation.PopModalAsync();     

    }

    private bool CompanyExists(string name, string symbol)
    {
        foreach (var company in Dictionaries.AllCompanies)
        {
            // Check either the company name or the symbol.
            if (company.Key.Equals(name, StringComparison.OrdinalIgnoreCase) ||
                company.Value.Equals(symbol, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private async void TestDownload_Clicked(object sender, EventArgs e)
    {
        btnTest.IsEnabled = false;

        string companyName = txtName.Text.Trim();
        string symbol = txtSymbol.Text.Trim().ToUpper();



        // Basic validation.
        if (companyName == "" || symbol == "" || pkCountry.SelectedIndex < 0)
                    {
            await DisplayAlert("Missing Information",
                "Please enter Company Name, Symbol and Country.",
                "OK");                      
        }

        string country = pkCountry.SelectedItem?.ToString() ?? "";

        if (CompanyExists(companyName, symbol))
        {
            await DisplayAlert(
            "Company Already Exists",
            $"{companyName} ({symbol}) already exists in the {country} list.","OK");

            btnSave.IsEnabled = false;
            return;
        }
        string provider = AppGlobals.ConfigurationManager.APIProvider;

        bool ok = await Services.DownloadService.DownloadFromProvider(provider,companyName, symbol);

        btnTest.IsEnabled = true;

        if (ok)
        {
            await DisplayAlert("Success",
                "Share price download succeeded.",
                "OK");

            btnSave.IsEnabled = true;
           
        }
        else
        {
            string message =$"Your API Providor may not support this symbol ({symbol}) or the symbol is invalid. Please check and try again.";
            await DisplayAlert("Download Failed",
                $"{message}",
                "OK");

            btnSave.IsEnabled = false;
        }
    }
      
}