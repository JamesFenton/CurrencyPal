using Newtonsoft.Json.Linq;
using Rates.Core.WriteModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Rates.Fetcher.Services
{
    public class OpenExchangeRatesService
    {
        private readonly string _openExchangeRatesKey;
        private readonly HttpClient _http;

        public OpenExchangeRatesService(string openExchangeRatesKey, HttpClient http)
        {
            _openExchangeRatesKey = openExchangeRatesKey;
            _http = http;
        }

        public async Task<List<Rate>> GetExchangeRates(IEnumerable<string> tickers)
        {
            var response = await _http.GetStringAsync("https://openexchangerates.org/api/latest.json?app_id=" + _openExchangeRatesKey);
            var sourceRates = JObject.Parse(response)["rates"] as JObject;

            var rates = tickers.Select(t => ConvertRate(t)).ToList();
            return rates;

            Rate ConvertRate(string ticker)
            {
                var source = ticker.Substring(0, 3);
                var dest = ticker.Substring(3, 3);

                var sourceRate = sourceRates[source].Value<double>();
                var destRate = sourceRates[dest].Value<double>();
                var crossRate = destRate / sourceRate;
                return new Rate(Guid.NewGuid(), ticker, DateTime.UtcNow, crossRate);
            }
        }
    }
}
