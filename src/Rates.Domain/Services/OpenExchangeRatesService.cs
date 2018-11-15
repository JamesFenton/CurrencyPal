using Newtonsoft.Json.Linq;
using Polly.Retry;
using Rates.Core;
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
    public class OpenExchangeRatesService : IRatesService
    {
        private readonly HttpClient _http = new HttpClient();
        private readonly RetryPolicy _retryPolicy;

        public OpenExchangeRatesService(Settings settings, RetryPolicy retryPolicy)
        {
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", settings.OpenExchangeRatesApiKey);
            _retryPolicy = retryPolicy;
        }

        public async Task<List<Rate>> GetRates(IEnumerable<string> tickers)
        {
            var json = await _retryPolicy.ExecuteAsync(() => _http.GetStringAsync("https://openexchangerates.org/api/latest.json"));
            var sourceRates = JObject.Parse(json)["rates"] as JObject;

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
