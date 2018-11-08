using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rates.Core
{
    public static class Constants
    {
        public static string DatabaseConnectionString => Environment.GetEnvironmentVariable("RATES_DB_CONNECTIONSTRING");
        public static string CoinMarketCapApiKey => Environment.GetEnvironmentVariable("CMC_API_KEY");
        public static string OpenExchangeRatesAppId => Environment.GetEnvironmentVariable("OPENEXCHANGERATES_APPID");

        public static readonly string[] FiatTickers = 
        {
            "USDZAR",
            "GBPZAR",
            "EURZAR",
            "ZARMUR"
        };

        public static readonly string[] MetalsTickers = 
        {
            "XAUUSD",
            "XAGUSD"
        };

        public static readonly string[] CryptoTickers =
        {
            "BTCUSD",
            "ETHUSD",
            "ZECUSD",
            "LTCUSD",
            "NEOUSD",
            "XLMUSD"
        };

        public static readonly string[] StockTickers =
        {
            "SPXUSD",
        };

        public static readonly string[] AllTickers = FiatTickers
            .Concat(StockTickers)
            .Concat(MetalsTickers)
            .Concat(CryptoTickers)
            .ToArray();
    }
}
