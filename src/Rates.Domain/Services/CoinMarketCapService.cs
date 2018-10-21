using Newtonsoft.Json.Linq;
using Rates.Core.WriteModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Rates.Domain.Services
{
    public class CoinMarketCapService
    {
        private readonly HttpClient _http = new HttpClient();

        public CoinMarketCapService(string apiKey)
        {
            _http.DefaultRequestHeaders.Add("X-CMC_PRO_API_KEY", apiKey);
        }

        public async Task<List<Rate>> GetCryptoCurrencies(IEnumerable<string> tickers)
        {
            var symbols = tickers.ToDictionary(t => t, t => GetCoinMarketCapSymbol(t));
            var symbolsQueryString = string.Join(",", symbols.Values);

            var url = $"https://pro-api.coinmarketcap.com/v1/cryptocurrency/quotes/latest?symbol={symbolsQueryString}";
            
            var response = JObject.Parse(await _http.GetStringAsync(url));

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
