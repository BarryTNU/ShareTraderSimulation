using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareTrader
{
    public static class Dictionaries
    {
        // Dictionary of companies and their stock symbols
        public static Dictionary<string, string> AllCompanies = new Dictionary<string, string>
            {
{"BHP","BHP,ax"},
{"Commonwealth Bank","CBA,ax"},
{"Endeavour Group","EDV,ax"},
{"MFF Capital Investments","MFF,ax"},
{"Santana Minerals Ltd","SMI,ax"},
{"Seek Minerals Ltd","SEK,ax"},
{"Washington H Soul Pattinson & Co Ltd","SOL,ax"},
{"XERO Ltd","XRO,ax"},
{"CSL","CSL,ax"},
{"National Australia Bank","NAB,ax"},
{"Westpac Banking Corporation","WBC,ax"},
{"ANZ Group Holdings","ANZ,ax"},
{"Macquarie Group","MQG,ax"},
{"Wesfarmers","WES,ax"},
{"Woodside Energy","WDS,ax"},
{"Telstra","TLS,ax"},
{"Aussie Broadband Ltd","ABB,ax" },
{"Woolworths Group","WOW,ax"},
{"Rio Tinto (Australia)","RIO,ax"},
{"Fortescue","FMG,ax"},
{"Goodman Group","GMG,ax"},
{"Transurban","TCL,ax"},
{"Aristocrat Leisure","ALL,ax"},
{"Santos","STO,ax"},
{"QBE Insurance","QBE,ax"},
{"Origin Energy","ORG,ax"},
{"Suncorp Group","SUN,ax"},
{"Coles Group","COL,ax"},
{"James Hardie","JHX,ax"},
{"Cochlear","COH,ax"},
{"ResMed","RMD,ax"},
{"South32","S32,ax"},
{"Insurance Australia Group","IAG,ax"},
{"APA Group","APA,ax"},
{"Apple","AAPL,USA"},
{"Microsoft","MSFT,USA"},
{"NVIDIA","NVDA,USA"},
{"Amazon","AMZN,USA"},
{"Alphabet (Google)","GOOGL,USA"},
{"Meta (Facebook)","META,USA"},
{"Tesla","TSLA,USA"},
{"Berkshire Hathaway","BRK.B,USA"},
{"JPMorgan Chase","JPM,USA"},
{"Visa","V,USA"},
{"Mastercard","MA,USA"},
{"Johnson & Johnson","JNJ,USA"},
{"Procter & Gamble","PG,USA"},
{"Coca-Cola","KO,USA"},
{"Heron Theraoeutics Ltd","HRTX.USA" },
{"PepsiCo","PEP,USA"},
{"Walmart","WMT,USA"},
{"Costco Wholesale","COST,USA"},
{"Home Depot","HD,USA"},
{"McDonald's","MCD,USA"},
{"Exxon Mobil","XOM,USA"},
{"Chevron","CVX,USA"},
{"Broadcom","AVGO,USA"},
{"Oracle","ORCL,USA"},
{"Netflix","NFLX,USA"},
{"Adobe","ADBE,USA"},
{"Salesforce","CRM,USA"},
{"Intel","INTC,USA"},
{"AMD","AMD,USA"},
{"AT&T","T.USA" },
{"Berkshire Hathaway-B", "BRK-B.us" },
{"Berkshire Hathaway-A", "BRK-A.us" },
{"Qualcomm","QCOM,USA"},
{"Cisco Systems","CSCO,USA"},
{"AstraZeneca","AZN,UK"},
{"Shell","SHEL,UK"},
{"HSBC Holdings","HSBA,UK"},
{"Unilever","ULVR,UK"},
{"BP","BP,UK"},
{"GSK","GSK,UK"},
{"Diageo","DGE,UK"},
{"Rio Tinto (UK)","RIO,UK"},
{"British American Tobacco","BATS,UK"},
{"Lloyds Banking Group","LLOY,UK"},
{"Barclays","BARC,UK"},
{"NatWest Group","NWG,UK"},
{"Prudential","PRU,UK"},
{"Legal & General","LGEN,UK"},
{"Aviva","AV,UK"},
{"RELX","REL,UK"},
{"BAE Systems","BA,UK"},
{"Vodafone","VOD,UK"},
{"Tesco","TSCO,UK"},
{"Sainsbury","SBRY,UK"},
{"Compass Group","CPG,UK"},
{"National Grid","NG,UK"},
{"Imperial Brands","IMB,UK"},
{"Rolls-Royce Holdings","RR,UK"},
{"Experian","EXPN,UK"},
{"London Stock Exchange Group","LSEG,UK"},
{"Anglo American","AAL,UK"},
{"Pearson","PSON,UK"},
{"Whitbread","WTB,UK"},
{"Burberry","BRBY,UK"},
{"Fisher & Paykel Healthcare","FPH,nz"},
{"Auckland International Airport","AIA,nz"},
{"Spark New Zealand","SPK,nz"},
{"Meridian Energy","MEL,nz"},
{"Contact Energy","CEN,nz"},
{"Mainfreight","MFT,nz"},
{"EBOS Group","EBO,nz"},
{"Infratil","IFT,nz"},
{"Mercury","MCY,nz"},
{"Fletcher Building","FBU,nz"},
{"Genesis Energy","GNE,nz"},
{"Ryman Healthcare","RYM,nz"},
{"Summerset Group","SUM,nz"},
{"Port of Tauranga","POT,nz"},
{"Chorus","CNU,nz"},
{"Kiwi Property Group","KPG,nz"},
{"Precinct Properties","PCT,nz"},
{"Skellerup Holdings","SKL,nz"},
{"Vista Group","VGL,nz"},
{"Freightways","FRW,nz"}
            };

        public static Dictionary<string, string> USCompanies = new Dictionary<string, string>
            {
                { "Apple", "AAPL.us" },
                { "Microsoft", "MSFT.us" },
                { "Amazon", "AMZN.us" },
                { "Alphabet (Google)", "GOOGL.us" },
                { "Meta (Facebook)", "META.us" },
                {"Heron Therapeutics Ltd","HRTX.USA" },
                {"AT&T","T.USA" },
                { "Tesla", "TSLA.us" },
                { "NVIDIA", "NVDA.us" },
                { "Berkshire Hathaway", "BRK-B.us" },
                { "Johnson & Johnson", "JNJ.us" },
                { "JPMorgan Chase", "JPM.us" }
            };

        // Changed the access modifier of the UKCompanies field to public
        public static Dictionary<string, string> UKCompanies = new Dictionary<string, string>
            {
                { "BP", "BP.uk" },
                { "Lloyds", "LLOY.uk" },
                { "Tesco", "TSCO.uk" },
                { "Vodafone", "VOD.uk" },
                { "Unilever", "ULVR.uk" }
            };

        public static Dictionary<string, string> NZCompanies = new Dictionary<string, string>
            {
                { "Auckland Airport", "AIA.nz" },
                { "Air New Zealand", "AIR.nz" },
                { "Meridian Energy", " MEL.nz" }
            };


        public static Dictionary<string, string> AUSCompanies = new Dictionary<string, string>
            {
                {"BHP","BHP,ax"},
                {"Commonwealth Bank","CBA,ax"},
                {"Endeavour Group","EDV.ax"},
                {"MFF Capital Investments","MFF.ax"},
                {"Santana Minerals Ltd","SMI.ax"},
                {"Seek Minerals Ltd","SEK.ax"},
                {"Washington H Soul Pattinson & Co Ltd","SOL.ax"},
                {"XERO Ltd","XRO.ax"},
                {"Rio Tinto", "RIO.ax" },
                {"ANZ Bank", "ANZ.ax" },
                {"National Australia Bank", "NAB.ax" },
                {"Telstra", "TLS.ax" },
                {"Wesfarmers", "WES.ax" },
                {"CSL", "CSL.ax" },
                {"Macquarie Group", "MQG.ax" },
                {"Woodside Energy", "WDS.ax" },
                {"Fortescue Metals", "FMG.ax" },
                {"Transurban", "TCL.ax" },
                {"Origin Energy", "ORG.ax" },
                {"Santos", "STO.ax" },
                {"QBE Insurance", "QBE.ax" }
            };

      
   public static readonly Dictionary<string, string> IndicatorDescriptions = new()
{
    { "MACD",
      "Moving Average Convergence Divergence (MACD) measures momentum by comparing the 12-day and 26-day Exponential Moving Averages (EMAs). A 9-day EMA of the MACD forms the signal line. Crossovers between the MACD and signal line can indicate potential buy or sell signals, while the distance between them reflects the strength of momentum." },

    { "RSI",
      "Relative Strength Index (RSI) measures the speed and magnitude of recent price movements on a scale from 0 to 100. Values above 70 may indicate an overbought market, while values below 30 may indicate an oversold market. RSI is commonly used to identify momentum shifts and possible reversal points." },

    { "ADX",
      "Average Directional Index (ADX) measures the strength of a trend, regardless of whether prices are rising or falling. Values below 20 often indicate a weak or sideways market, while values above 25–30 suggest a strengthening trend. ADX is best used together with other indicators that identify trend direction." },

    { "OBV",
      "On Balance Volume (OBV) combines price movement and trading volume to estimate whether money is flowing into or out of a share. Rising OBV suggests accumulation by buyers, while falling OBV suggests distribution by sellers. Divergences between OBV and price can provide early warning of potential trend changes." },

    { "ATR",
      "Average True Range (ATR) measures market volatility by calculating the average daily trading range over a selected period. A rising ATR indicates increasing volatility, while a falling ATR indicates quieter market conditions. ATR measures volatility only—it does not indicate trend direction." },

    { "ROC",
      "Rate of Change (ROC) measures the percentage change in price over a specified number of trading days. Positive values indicate upward momentum, while negative values indicate downward momentum. ROC is useful for spotting accelerating or weakening price trends and potential momentum reversals." },

    { "WILL",
      "Williams %R is a momentum indicator that measures overbought and oversold conditions on a scale from 0 to -100. Readings above -20 often indicate overbought conditions, while readings below -80 often indicate oversold conditions. It is similar to the Stochastic Oscillator but uses an inverted scale. " },

    { "STOC",
      "The Stochastic Oscillator compares the closing price with the recent trading range over a selected period. Values range from 0 to 100, with readings above 80 often considered overbought and below 20 considered oversold. Crossovers between the %K and %D lines may signal potential changes in momentum. Stochastic %K is mathematically equivalent to the Williams %R indicator, displayed on a 0 to 100 scale." },

    { "BOLL",
      "Bollinger Bands measure price volatility by plotting upper and lower bands around a moving average. The bands widen during periods of high volatility and narrow during quieter markets. Prices touching or moving outside the bands can signal unusually strong moves, but should be confirmed with other indicators." },

    { "MAS",
      "Moving Averages smooth daily price fluctuations to reveal the underlying market trend. Short-term averages respond quickly to price changes, while longer-term averages highlight the broader trend. Crossovers between moving averages are commonly used to identify potential trend changes." },

    { "VOL",
      "Volume shows the number of shares traded during each trading period. Rising volume confirms stronger buying or selling interest, while low volume often indicates weaker conviction behind price movements. Significant price moves accompanied by high volume are generally considered more reliable than moves on low volume." }
};

        public static readonly Dictionary<string, string> ChartTypes = new()
{
    { "Average Directional Index", "ADX" },
    { "Bollinger Bands", "BOLL" },
    { "Moving Average Slope", "MAS" },
    { "Moving Average Convergence Divergence", "MACD" },
    { "Relative Strength Index", "RSI" },
    { "Stochastic Oscillator", "STOC" },
    { "Williams %R", "WILL" },
    { "Rate of Change", "ROC" },
    { "Average True Range", "ATR" },
    { "Volume", "VOL" },
    { "On Balance Volume", "OBV" }
};


    }
}
