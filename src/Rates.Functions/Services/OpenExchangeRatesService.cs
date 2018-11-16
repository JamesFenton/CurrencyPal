using Newtonsoft.Json.Linq;
using Rates.Functions.WriteModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Rates.Functions.Services
{
    public class OpenExchangeRatesService : IRatesService
    {
        private readonly HttpClient _http = new HttpClient();

        public Rate[] Rates => new[]
        {
            Rate.USDZAR,
            Rate.GBPZAR,
            Rate.EURZAR,
            Rate.ZARMUR,
            Rate.XAUUSD,
            Rate.XAGUSD,
        };

        public OpenExchangeRatesService(Settings settings)
        {
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", settings.OpenExchangeRatesApiKey);
        }

        public async Task<IEnumerable<RateEntity>> GetRates()
        {
            var json = await _http.GetStringAsync("https://openexchangerates.org/api/latest.json");
            var sourceRates = JObject.Parse(json)["rates"] as JObject;

            var rates = Rates.Select(t => ConvertRate(t));
            return rates;

            RateEntity ConvertRate(Rate rate)
            {
                var ticker = rate.Ticker;
                var source = ticker.Substring(0, 3);
                var dest = ticker.Substring(3, 3);

                var sourceRate = sourceRates[source].Value<double>();
                var destRate = sourceRates[dest].Value<double>();
                var crossRate = destRate / sourceRate;
                return new RateEntity(ticker, DateTimeOffset.UtcNow, crossRate);
            }
        }
    }
}
