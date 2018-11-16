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

        public Rate[] Rates => new[]
        {
            Rate.BTCUSD,
            Rate.ETHUSD,
            Rate.ZECUSD,
            Rate.LTCUSD,
            Rate.NEOUSD,
            Rate.XLMUSD,
        };

        public CoinMarketCapService(Settings settings)
        {
            _http.DefaultRequestHeaders.Add("X-CMC_PRO_API_KEY", settings.CoinMarketCapApiKey);
        }
        
        public async Task<IEnumerable<RateEntity>> GetRates()
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
