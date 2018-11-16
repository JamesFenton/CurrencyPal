using Newtonsoft.Json.Linq;
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

        public string[] Tickers => Constants.CryptoTickers;

        public CoinMarketCapService(Settings settings)
        {
            _http.DefaultRequestHeaders.Add("X-CMC_PRO_API_KEY", settings.CoinMarketCapApiKey);
        }
        
        public async Task<List<Rate>> GetRates()
        {
            var symbols = Tickers.ToDictionary(t => t, t => GetCoinMarketCapSymbol(t));
            var symbolsQueryString = string.Join(",", symbols.Values);

            var url = $"https://pro-api.coinmarketcap.com/v1/cryptocurrency/quotes/latest?symbol={symbolsQueryString}";
            var json = await _http.GetStringAsync(url);
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
