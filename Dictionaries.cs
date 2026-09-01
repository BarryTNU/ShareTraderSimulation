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
    }
}

