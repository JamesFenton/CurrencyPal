using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rates.Functions
{
    public class Rate
    {
        // forex
        public static Rate USDZAR => new Rate("USDZAR", "USDZAR", null); 
        public static Rate GBPZAR => new Rate("GBPZAR", "GBPZAR", null); 
        public static Rate EURZAR => new Rate("EURZAR", "EURZAR", null); 
        public static Rate ZARMUR => new Rate("ZARMUR", "ZARMUR", null);
        public static Rate XAUUSD => new Rate("XAUUSD", "XAUUSD", null); 
        public static Rate XAGUSD => new Rate("XAGUSD", "XAGUSD", null);
        // crypto
        public static Rate BTCUSD => new Rate("BTCUSD", "BTCUSD", null); 
        public static Rate ETHUSD => new Rate("ETHUSD", "ETHUSD", null);
        public static Rate ZECUSD => new Rate("ZECUSD", "ZECUSD", null);
        public static Rate LTCUSD => new Rate("LTCUSD", "LTCUSD", null);
        public static Rate NEOUSD => new Rate("NEOUSD", "NEOUSD", null);
        public static Rate XLMUSD => new Rate("XLMUSD", "XLMUSD", null);
        // stocks
        public static Rate VOOUSD => new Rate("VOOUSD", "S&P 500", "https://www.bloomberg.com/quote/VOO:US");

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
