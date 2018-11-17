using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rates.Functions
{
    public class Rate
    {
        // forex
        public static Rate USDZAR => new Rate("USDZAR", "USDZAR", "https://www.xe.com/currencycharts/?from=USD&to=ZAR"); 
        public static Rate GBPZAR => new Rate("GBPZAR", "GBPZAR", "https://www.xe.com/currencycharts/?from=GBP&to=ZAR"); 
        public static Rate EURZAR => new Rate("EURZAR", "EURZAR", "https://www.xe.com/currencycharts/?from=EUR&to=ZAR"); 
        public static Rate ZARMUR => new Rate("ZARMUR", "ZARMUR", "https://www.xe.com/currencycharts/?from=ZAR&to=MUR");
        // stocks
        public static Rate VOOUSD => new Rate("VOOUSD", "S&P 500", "https://www.bloomberg.com/quote/VOO:US");
        // metals
        public static Rate XAUUSD => new Rate("XAUUSD", "Gold", "https://www.xe.com/currencycharts/?from=XAU&to=USD"); 
        public static Rate XAGUSD => new Rate("XAGUSD", "Silver", "https://www.xe.com/currencycharts/?from=XAG&to=USD");
        // crypto
        public static Rate BTCUSD => new Rate("BTCUSD", "BTCUSD", "https://coinmarketcap.com/currencies/bitcoin/"); 
        public static Rate ETHUSD => new Rate("ETHUSD", "ETHUSD", "https://coinmarketcap.com/currencies/ethereum/");
        public static Rate ZECUSD => new Rate("ZECUSD", "ZECUSD", "https://coinmarketcap.com/currencies/zcash/");
        public static Rate LTCUSD => new Rate("LTCUSD", "LTCUSD", "https://coinmarketcap.com/currencies/litecoin/");
        public static Rate NEOUSD => new Rate("NEOUSD", "NEOUSD", "https://coinmarketcap.com/currencies/neo/");
        public static Rate XLMUSD => new Rate("XLMUSD", "XLMUSD", "https://coinmarketcap.com/currencies/stellar/");

        public static Rate[] All => typeof(Rate)
            .GetProperties()
            .Where(p => p.PropertyType == typeof(Rate))
            .Select(p => (Rate)p.GetValue(null))
            .ToArray();

        public string Ticker { get; }
        public string Name { get; }
        public string Href { get; }

        private Rate(string ticker, string name, string href)
        {
            Ticker = ticker;
            Name = name;
            Href = href;
        }
    }
}
