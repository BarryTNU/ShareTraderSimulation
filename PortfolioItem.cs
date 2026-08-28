using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareTrader;

public class Port_folio_Item
{
    public string CompanyName { get; set; } = "";
    public string Symbol { get; set; } = "";

    public decimal BuyPrice { get; set; }

    public int Shares { get; set; }

    public decimal CurrentPrice { get; set; }

    public decimal Value
    {
        get { return Shares * CurrentPrice; }
    }

    public decimal Profit
    {
        get { return (CurrentPrice - BuyPrice) * Shares; }
    }
}
