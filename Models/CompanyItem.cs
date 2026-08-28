using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareTrader.Models;

public class CompanyItem
{
    public string Symbol { get; set; } = "";
    public string CompanyName { get; set; } = "";

    public int Shares { get; set; }

    public decimal BuyPrice { get; set; }

    public decimal CurrentPrice { get; set; }

    public decimal Value => Shares * CurrentPrice;

    public decimal Profit => (CurrentPrice - BuyPrice) * Shares;
}
