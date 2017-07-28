﻿using Newtonsoft.Json.Linq;
using Rates.Core;
using Rates.Core.WriteModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Rates.Fetcher
{
    public class RatesFetcher
    {
        private readonly string _openExchangeRatesKey;
        private readonly HttpClient _http;

        public RatesFetcher(string openExchangeRatesKey, HttpClient http)
        {
            _openExchangeRatesKey = openExchangeRatesKey;
            _http = http;
        }

        public async Task<List<Rate>> GetRates()
        {
            var tasks = new List<Task<List<Rate>>>
            {
                GetExchangeRates(),
                GetBitcoinZar(),
                GetCryptoCurrencies(),
            };

            await Task.WhenAll(tasks);

            var rates = tasks.SelectMany(t => t.Result).ToList();

            return rates;
        }

        private async Task<List<Rate>> GetExchangeRates()
        {
            var response = await _http.GetStringAsync("https://openexchangerates.org/api/latest.json?app_id=" + _openExchangeRatesKey);
            var sourceRates = JObject.Parse(response)["rates"] as JObject;

            var rates = Constants.FiatTickers.Select(t => ConvertRate(t)).ToList();
            return rates;

            Rate ConvertRate(string ticker)
            {
                var source = ticker.Substring(0, 3);
                var dest = ticker.Substring(3, 3);

                var sourceRate = sourceRates[source].Value<double>();
                var destRate = sourceRates[dest].Value<double>();
                var crossRate = destRate / sourceRate;
                return new Rate(Guid.NewGuid(), ticker, DateTime.UtcNow, crossRate);
            }
        }
        
        private async Task<List<Rate>> GetBitcoinZar()
        {
            var response = await _http.GetStringAsync("https://api.mybitx.com/api/1/ticker?pair=XBTZAR");
            var responseObject = JObject.Parse(response) as JObject;
            var rate = responseObject["last_trade"].Value<double>();

            return new List<Rate>
            {
                new Rate(Guid.NewGuid(), "BTCZAR", DateTime.UtcNow, rate)
            };
        }

        private async Task<List<Rate>> GetCryptoCurrencies()
        {
            var tasks = Constants.CryptoTickers.Select(ticker => GetRate(ticker)).ToList();

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
                    default:
                        throw new ArgumentException("Invalid ticker: " + ticker);
                }
                var response = await _http.GetStringAsync("https://api.coinmarketcap.com/v1/ticker/" + coinmarketcapTicker);
                var rateReponse = JArray.Parse(response).First();
                var rate = rateReponse["price_usd"].Value<double>();
                return new Rate(Guid.NewGuid(), ticker, DateTime.UtcNow, rate);
            }
        }

        private async Task<List<Rate>> GetShapeshift()
        {
            var tasks = new List<Task<Rate>>
            {
                GetRate("BTCETH"),
                GetRate("BTCZEC")
            };

            await Task.WhenAll(tasks);

            return tasks.Select(t => t.Result).ToList();

            async Task<Rate> GetRate(string ticker)
            {
                string shapeshiftTicker;
                switch (ticker)
                {
                    case "BTCETH":
                        shapeshiftTicker = "btc_eth";
                        break;
                    case "BTCZEC":
                        shapeshiftTicker = "btc_zec";
                        break;
                    default:
                        throw new ArgumentException("Invalid ticker: " + ticker);
                }
                var response = await _http.GetStringAsync($"http://shapeshift.io/rate/{shapeshiftTicker}");
                var rate = JObject.Parse(response)["rate"].ToObject<double>();
                return new Rate(Guid.NewGuid(), ticker, DateTime.UtcNow, rate);
            }
        }
    }
}