using Newtonsoft.Json.Linq;
using Polly.Retry;
using Rates.Functions.WriteModel;
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
        private readonly HttpClient _http = new HttpClient();
        private readonly RetryPolicy _retryPolicy;

        public Rate[] Rates => new[]
        {
            Rate.BTCUSD,
            Rate.ETHUSD,
            Rate.ZECUSD,
            Rate.LTCUSD,
            Rate.NEOUSD,
            Rate.XLMUSD,
        };

        public CoinMarketCapService(Settings settings, RetryPolicy retryPolicy)
        {
            _http.DefaultRequestHeaders.Add("X-CMC_PRO_API_KEY", settings.CoinMarketCapApiKey);
            _retryPolicy = retryPolicy;
        }

        public Task<IEnumerable<RateEntity>> GetRates()
        {
            return _retryPolicy.ExecuteAsync(() => Get());
        }

        private async Task<IEnumerable<RateEntity>> Get()
        {
            var symbols = Rates.ToDictionary(t => t, t => GetCoinMarketCapSymbol(t.Ticker));
            var symbolsQueryString = string.Join(",", symbols.Values);

            var url = $"https://pro-api.coinmarketcap.com/v1/cryptocurrency/quotes/latest?symbol={symbolsQueryString}";
            var json = await _http.GetStringAsync(url);
            var response = JObject.Parse(json);

            var rates = symbols.Keys.Select(s =>
            {
                var ticker = s.Ticker;
                var coinMarketCapSymbol = symbols[s];
                var rate = response["data"][coinMarketCapSymbol]["quote"]["USD"]["price"].Value<double>();
                return new RateEntity(ticker, DateTimeOffset.UtcNow, rate);
            });

            return rates;
        }
        
        private string GetCoinMarketCapSymbol(string ticker)
        {
            // BTCUSD -> BTC
            return ticker.Substring(0, 3);
        }
    }
}
