using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rates.Domain
{
    public class Settings
    {
        public string DatabaseConnectionString { get; set; }
        public string CoinMarketCapApiKey { get; set; }
        public string OpenExchangeRatesApiKey { get; set; }
    }
}
