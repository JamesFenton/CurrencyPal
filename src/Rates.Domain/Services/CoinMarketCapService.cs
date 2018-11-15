using Newtonsoft.Json.Linq;
using Polly.Retry;
using Rates.Core;
using Rates.Core.WriteModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Rates.Domain.Services
{
    public class CoinMarketCapService : IRatesService
    {
        private readonly HttpClient _http = new HttpClient();
        private readonly RetryPolicy _retryPolicy;

        public CoinMarketCapService(Settings settings, RetryPolicy retryPolicy)
        {
            _http.DefaultRequestHeaders.Add("X-CMC_PRO_API_KEY", settings.CoinMarketCapApiKey);
            _retryPolicy = retryPolicy;
        }

        public async Task<List<Rate>> GetRates(IEnumerable<string> tickers)
        {
            var symbols = tickers.ToDictionary(t => t, t => GetCoinMarketCapSymbol(t));
            var symbolsQueryString = string.Join(",", symbols.Values);

            var url = $"https://pro-api.coinmarketcap.com/v1/cryptocurrency/quotes/latest?symbol={symbolsQueryString}";
            
            var json = await _retryPolicy.ExecuteAsync(() => _http.GetStringAsync(url));
            var response = JObject.Parse(json);

            var rates = symbols.Keys.Select(s =>
            {
                var ticker = s;
                var coinMarketCapSymbol = symbols[s];
                var rate = response["data"][coinMarketCapSymbol]["quote"]["USD"]["price"].Value<double>();
                return new Rate(ticker, DateTimeOffset.UtcNow, rate);
            });

            return rates.ToList();

            string GetCoinMarketCapSymbol(string ticker)
            {
                // BTCUSD -> BTC
                return ticker.Substring(0, 3);
            }
        }
    }
}
