using Newtonsoft.Json.Linq;
using Rates.Functions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Rates.Functions.Services
{
    public class CoinMarketCapService : IRatesService
    {
        private readonly HttpClient _http;

        public CoinMarketCapService(HttpClient http)
        {
            _http = http;
        }

        public async Task<IEnumerable<RateEntity>> GetRates(IEnumerable<Rate> rates)
        {
            var ratesToFetch = rates.Where(r => r.Source == RateSource.CoinMarketCap);

            var queryString = string.Join(",", ratesToFetch.Select(r => r.SourceSymbol));

            var url = $"/v1/cryptocurrency/quotes/latest?symbol={queryString}";
            var json = await _http.GetStringAsync(url);
            var response = JObject.Parse(json);

            var values = ratesToFetch.Select(s =>
            {
                var ticker = s.Ticker;
                var coinMarketCapSymbol = s.SourceSymbol;
                var rate = response["data"][coinMarketCapSymbol]["quote"]["USD"]["price"].Value<double>();
                return new RateEntity(ticker, DateTimeOffset.UtcNow, rate);
            });

            return values;
        }
    }
}
