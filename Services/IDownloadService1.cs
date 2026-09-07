namespace ShareTrader.Services
{
    internal interface IDownloadService1
    {
        static abstract string BuildApiUrl(string provider, string symbol);
        static abstract string ConvertSymbol(string symbol, string provider);
        static abstract bool DataIsValid(string provider, string data);
        static abstract Task<bool> DownloadData(string symbol, string companyName);
        static abstract Task<bool> DownloadFromProvider(string provider, string companyName, string symbol, string? busyMessage = null);
        static abstract Task<bool> SaveData(string provider, string CompanyName, string tData);
        static abstract Task<bool> UpdateSharePrices();
    }
}