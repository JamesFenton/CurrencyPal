using Newtonsoft.Json.Linq;
using Rates.Core.WriteModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Rates.Domain.Services
{
    public class OpenExchangeRatesService
    {
        private readonly HttpClient _http = new HttpClient();

        public OpenExchangeRatesService(string apiKey)
        {
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", apiKey);
        }

        public async Task<List<Rate>> GetExchangeRates(IEnumerable<string> tickers)
        {
            var response = await _http.GetStringAsync("https://openexchangerates.org/api/latest.json");
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
                return new Rate(ticker, DateTimeOffset.UtcNow, crossRate);
            }
        }
    }
}
