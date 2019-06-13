using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rates.Functions
{
    public class Settings
    {
        public string DatabaseConnectionString { get; set; }
        public string CoinMarketCapApiKey { get; set; }
        public string OpenExchangeRatesApiKey { get; set; }
        public string IexApiKey { get; set; }
    }
}
