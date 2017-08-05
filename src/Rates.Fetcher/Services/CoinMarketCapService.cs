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
    public class CoinMarketCapService
    {
        private readonly HttpClient _http;

        public CoinMarketCapService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<Rate>> GetCryptoCurrencies(IEnumerable<string> tickers)
        {
            var tasks = tickers.Select(ticker => GetRate(ticker)).ToList();

            await Task.WhenAll(tasks);

            return tasks.Select(t => t.Result).ToList();

            async Task<Rate> GetRate(string ticker)
            {
                string coinmarketcapTicker;
                switch (ticker)
                {
                    case "BTCUSD":
                        coinmarketcapTicker = "bitcoin";
                        break;
                    case "ETHUSD":
                        coinmarketcapTicker = "ethereum";
                        break;
                    case "ZECUSD":
                        coinmarketcapTicker = "zcash";
                        break;
                    case "EOSUSD":
                        coinmarketcapTicker = "eos";
                        break;
                    case "SIGTUSD":
                        coinmarketcapTicker = "signatum";
                        break;
                    default:
                        throw new ArgumentException("Invalid ticker: " + ticker);
                }
                var response = await _http.GetStringAsync("https://api.coinmarketcap.com/v1/ticker/" + coinmarketcapTicker);
                var rateReponse = JArray.Parse(response).First();
                var rate = rateReponse["price_usd"].Value<double>();
                return new Rate(Guid.NewGuid(), ticker, DateTime.UtcNow, rate);
            }
        }
    }
}
