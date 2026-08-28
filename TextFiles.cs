using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareTrader
{
    public static class TextFiles

     
    {
        public static string crlf = Environment.NewLine;
        public static string Disclaimer =@"This is a suggestion only." +
            "Do not rely on it when deciding to Buy or Sell shares." +
            " Always do your own research," +
            " and make an informed decision.";

        public static string TradingStrategies()
        {
            return
        @" A commonly accepted conservative share trading policy is built around capital preservation first and profits second. The goal is steady long-term growth while avoiding large drawdowns.

CONSERVATIVE SHARE TRADING POLICY

1. Trade Only Quality Companies

Focus on:
• Large-cap, established businesses
• Consistent earnings
• Strong balance sheets
• Positive cash flow
• Reasonable debt

Examples in Australia might include companies in the ASX 20–50 rather than speculative micro-cap stocks.

2. Never Risk Too Much on One Trade

A common rule is to risk no more than 1–2% of your total portfolio value on any single trade.

Example:
Portfolio: $100,000
Maximum risk per trade: $1,000–$2,000

This prevents one bad trade from causing serious damage.

3. Always Use Stop Losses

Conservative traders define their exit before entering a trade.

Typical methods:
• Fixed percentage stop (5–8%)
• Technical stop below support or an SMA
• ATR volatility stop

Example:
Buy at $20.00
Stop at $18.80
Maximum loss: 6%

4. Avoid Overtrading

Conservative traders generally prefer:
• Fewer, high-quality trades
• Longer holding periods
• Confirmation before entry

This reduces:
• Emotional trading
• Brokerage costs
• False signals

5. Diversify

Avoid concentrating too much capital in one company or sector.

Typical guideline:
• No more than 10–15% invested in a single company.
• Spread investments across several industries.

6. Trade With the Trend

A classic conservative rule is to buy only when the long-term trend is upward.

Common measures include:
• 50-day SMA above the 200-day SMA
• Price above the 200-day moving average
• Higher highs and higher lows

7. Preserve Cash During Weak Markets

Conservative traders do not feel compelled to remain fully invested.

Many reduce exposure when:
• Major indices fall below their long-term averages
• Volatility increases sharply
• ADX weakens while downside momentum increases

Holding cash is considered a valid investment position.

8. Use Confirmation Indicators

Avoid entering a trade based on a single signal.

A typical conservative combination includes:
• Trend confirmation (SMA)
• Momentum confirmation (MACD and RSI)
• Strength confirmation (ADX)
• Volume confirmation (OBV or Volume)

9. Keep a Trading Journal

Record:
• Entry reason
• Exit reason
• Risk/reward ratio
• Emotional state
• Indicator conditions

Keeping a journal is one of the biggest differences between disciplined and emotional traders.

10. Prioritize Risk/Reward

Many conservative traders require a minimum risk/reward ratio of 1:2.

Example:
Risking $1.00 to potentially earn $2.00 or more.

Example Conservative Entry Model

Buy only when:
• Price > SMA30
• SMA7 > SMA20 > SMA30
• ADX > 20
• MACD crosses above zero
• Volume is above average

Exit when:
• SMA7 crosses below SMA20
• ADX weakens significantly
• Stop loss is triggered

THE MOST IMPORTANT CONSERVATIVE RULE

SURVIVE FIRST.

Most successful long-term traders are not the ones who make the biggest gains; they are the ones who avoid catastrophic losses.

Remember:
A portfolio that loses 50% must gain 100% just to recover.

Well-known conservative investing and trading philosophies include:
• Warren Buffett – Capital preservation and quality businesses.
• Benjamin Graham – Margin of safety.
• William O'Neil – Trend, earnings, and volume confirmation.";
        }



        public static string Manual()
        { 
            return
@" When first run the trading engine is blank; It must be populated with your Portfolio details.

First, Add Cash to the Bank, so you can buy shares.
    • 	Click 'Bank', and select 'Deposit'. An input panel opens for you to add funds to your account.
        This is virtual cash, so splash out: Enter $100,000.

Next, Add shares to your portfolio.
    • 	Click Portfolio button, and select 'Add Company'.
        A selector box opens for you to choose a company.
        Confirm your selection, and the company is added to the Portfolio.
        o You can only enter one company at a time. Repeat the action to add more companies.

Then, Buy some shares:
    • 	Hover the mouse on the Buy or Sell Shares button and select Buy.
        o A list of your Share Portfolio drops down: Select a company and enter the number of shares to buy.
           Confirm your choice, and the shares are added to your portfolio. The Bank Balance is adjusted accordingly.

This completes the setup. You can buy or sell shares at any time.

Left click on any company in the Portfolio panel, and an analysis panel displays.
showing details of the previous 30-day trading results.

Use this to make your decision as to Buy, Sell, or Hold.

A suggestion is offered, but don’t take this as gospel; Make your own decisions.

To remove a company from your portfolio, Click Portfolio button, and select 'Remove Company'.
        A selector box opens for you to choose a company to remove. 
        You cannot delete a company if you still hold shares in it.

Disclaimer: Trading Engine is a tool to help you make your own decisions.It might not be infallible.

By using Trading Engine, you specifically absolve the author of liability for any harm or losses incurred from using this tool.
";
        }

        public static string StrongBuy()
        {
            string crlf = Environment.NewLine;

            return
                "STRONG BUY" + crlf  +
                "The technical indicators are strongly positive, " +
                "with good upward momentum and favourable trend conditions." +
                crlf + crlf +
                "Consider buying if the current price is acceptable " +
                "and the level of risk suits your trading strategy." +
                crlf + crlf +
                Disclaimer;
        }
        public static string MediumBuy()
        {
            string crlf = Environment.NewLine;

            return
                "MEDIUM BUY" +  crlf +
                "The technical indicators are generally positive, " +
                "with evidence of an improving trend and reasonable " +
                "upward momentum." +
                crlf + crlf +
                "Consider buying if the current price is favourable " +
                "and the level of risk suits your trading strategy." +
                crlf + crlf +
                Disclaimer;
        }
        public static string Buy()
        {
            string crlf = Environment.NewLine;

            return
                "POSSIBLE BUY" + crlf  +
                "The technical indicators are showing a positive " +
                "outlook, although some indicators may not yet be " +
                "strongly aligned." +
                crlf + crlf +
                "A purchase may be appropriate if the current price " +
                "offers reasonable value and the risk suits your " +
                "trading strategy." +
                crlf + crlf +
                Disclaimer;
        }
        
           public static string Hold()
        {
            string crlf = Environment.NewLine;

            return
                "HOLD" + crlf +
                "The technical indicators are mixed or relatively neutral, " +
                "with no sufficiently strong signal to justify a new " +
                "buy or sell position at this time." +
                crlf + crlf +
                "Consider holding the current position and waiting for " +
                "a clearer trend to develop before taking further action." +
                crlf + crlf +
                Disclaimer;
        }
       
       public static string StrongSell()
        {
            string crlf = Environment.NewLine;

            return
                "STRONG SELL" + crlf  +
                "The technical indicators are strongly negative, with " +
                "significant evidence of weakening momentum and a " +
                "deteriorating downward trend." +
                crlf + crlf +
                "Consider selling or reducing the position, particularly " +
                "if the current price provides a reasonable opportunity " +
                "to limit potential losses." +
                crlf + crlf +
                Disclaimer;
        }

        public static string MediumSell()
        {
            string crlf = Environment.NewLine;

            return
                "MEDIUM SELL" + crlf +
                "The technical indicators are becoming increasingly " +
                "negative, with signs of weakening momentum and a " +
                "possible deterioration in the current trend." +
                crlf + crlf +
                "Consider reducing or selling the position if the " +
                "current price is favourable and the risk suits your " +
                "trading strategy." +
                crlf + crlf +
                Disclaimer;
        }

        public static string Sell()
        {
            string crlf = Environment.NewLine;

            return
                "POSSIBLE SELL" + crlf +
                "The technical indicators are showing a negative outlook, " +
                "with evidence of weakening momentum or deteriorating " +
                "trend conditions." +
                crlf + crlf +
                "Consider selling if the current price is favourable " +
                "and the level of risk suits your trading strategy." +
                crlf + crlf +
                Disclaimer;
        }
    }

    }





